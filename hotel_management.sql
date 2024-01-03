create database hotel_management
drop database hotel_management
go
use hotel_management
go
set dateformat dmy

create table staff
(
    staff_id char(5) not null primary key,
    full_name nvarchar(45) not null,
    position nvarchar(45) not null,
    contact_number nvarchar(10) not null unique,
    email nvarchar(45) unique,
    address nvarchar(100) not null,
    birthday date not null,
    gender nvarchar(6) not null,
    salary money,
    deleted bit default 0,
    deleted_date datetime,
    constraint chk_gender_staff check (gender in ('male', 'female')),
)

create table account
(
    username nvarchar(50) not null primary key,
    password nvarchar(100) not null,
    profile_picture varbinary(max),
    staff_id char(5) not null unique,
    status bit default 0,
)

create table customer
(
    customer_id char(5) not null primary key,
    full_name nvarchar(45) not null,
    contact_number nvarchar(10) not null unique,
    email nvarchar(45) unique,
    address nvarchar(100) not null,
    gender nvarchar(6) not null,
    credit_card nvarchar(45) unique,
    id_proof nvarchar(45) not null unique,
    deleted bit default 0,
    deleted_date datetime,
    constraint chk_gender_customer check (gender in ('male', 'female')),
)

create table room
(
    room_id char(5) not null primary key,
    room_number char(3) not null,
    notes nvarchar(45),
    room_type_id char(5) not null,
    deleted bit default 0,
    deleted_date datetime,
)

create table room_type
(
    room_type_id char(5) not null primary key,
    room_type_name nvarchar(20) not null unique,
    capacity int not null,
    bed_amount int not null,
    room_price money not null,
    room_type_desc nvarchar(100) not null,
    room_type_img varbinary(max),
    deleted bit default 0,
    deleted_date datetime,
)

create table service
(
    service_id char(5) not null primary key,
    service_name nvarchar(20) not null unique,
    service_type nvarchar(20) not null,
    service_price money not null,
    deleted bit default 0,
    deleted_date datetime,
)

create table invoice
(
    invoice_id char(5) not null primary key,
    customer_id char(5) not null,
    staff_id char(5) not null,
    invoice_date smalldatetime not null,
    total_amount money,
    payment_type nvarchar(20) not null,
    deleted bit default 0,
    deleted_date datetime,
    constraint chk_payment_type_booking check (payment_type in ('Cash', 'Credit card')),
)

create table booking
(
    booking_id char(5) not null primary key,
    invoice_id char(5) not null,
    room_id char(5) not null,
    guest_quantity int not null,
    check_in_date smalldatetime not null,
    check_out_date smalldatetime not null,
    total_amount money,
    deleted bit default 0,
    deleted_date datetime,
    constraint chk_check_in_out_booking check (check_in_date <= check_out_date),
)

create table service_use
(
    invoice_id char(5) not null,
    service_id char(5) not null,
    service_quantity int,
    total_amount money,
    deleted bit default 0,
    deleted_date datetime,
    constraint su_booking_service_pk primary key (invoice_id, service_id),
)

alter table account add
    constraint acc_staff_id_fk foreign key (staff_id) references staff(staff_id)
go

alter table room add
    constraint r_room_type_id_fk foreign key (room_type_id) references room_type(room_type_id)
go

alter table invoice add
    constraint in_customer_id_fk foreign key (customer_id) references customer(customer_id)
go

alter table invoice add
    constraint in_staff_id_fk foreign key (staff_id) references staff(staff_id)
go

alter table booking add
    constraint b_invoice_id_fk foreign key (invoice_id) references invoice(invoice_id)
go

alter table booking add
    constraint b_room_id_fk foreign key (room_id) references room(room_id)
go

alter table service_use add
    constraint su_invoice_id_fk foreign key (invoice_id) references invoice(invoice_id)
go

alter table service_use add
    constraint su_service_id_fk foreign key (service_id) references service(service_id)
go



--// triggers //--
--check guest quantity--
create trigger CheckGuestQuantity on booking
for insert, update
as
begin
    if exists (select *
               from inserted i
               join room on room.room_id = i.room_id
               join room_type rt on room.room_type_id = rt.room_type_id
               where i.guest_quantity > rt.capacity)
    begin
        raiserror ('Guest quantity must less than or equal to room capacity', 16, 1)
        rollback transaction
        return
    end
end
go

--update service_use total amount--
create trigger InsertAmountService on service_use
for insert
as
begin
    -- Update service_use
    update su
    set su.total_amount = s.service_price * i.service_quantity
    from service_use su
    join inserted i on su.service_id = i.service_id
    join service s on i.service_id = s.service_id

    -- Update invoice
    update inv
    set inv.total_amount = inv.total_amount + i.total_amount
    from invoice inv
    join (
        select service_use.invoice_id, sum(service_use.total_amount) as total_amount
        from service_use
        join inserted
        on service_use.invoice_id = inserted.invoice_id and service_use.service_id = inserted.service_id
        group by service_use.invoice_id
    ) i on inv.invoice_id = i.invoice_id
end
go

--update service_use total amount--
create trigger UpdateAmountService on service_use
for update
as
begin
    -- Update service_use
    update su
    set su.total_amount = s.service_price * i.service_quantity
    from service_use su
    join inserted i on su.service_id = i.service_id
    join service s on i.service_id = s.service_id

    -- Update invoice
    update inv
    set inv.total_amount = inv.total_amount + i.total_amount
    from invoice inv
    join (
        select inserted.invoice_id,
               sum(inserted.service_quantity * s.service_price - d.service_quantity * s.service_price) as total_amount
        from inserted
        join deleted d on inserted.invoice_id = d.invoice_id and inserted.service_id = d.service_id
        join service s on inserted.service_id = s.service_id
        group by inserted.invoice_id
    ) i on inv.invoice_id = i.invoice_id
end
go

--update booking total amount--
create trigger InsertAmountBooking on booking
for insert
as
begin
    -- Update booking
    update b
    set b.total_amount = rt.room_price * datediff(day, i.check_in_date, i.check_out_date)
    from booking b
    join inserted i on b.booking_id = i.booking_id and b.room_id = i.room_id
    join room r on r.room_id = i.room_id
    join room_type rt on rt.room_type_id = r.room_type_id

    -- update invoice
    update inv
    set inv.total_amount = inv.total_amount + i.total_amount
    from invoice inv
    join (
        select booking.invoice_id, sum(booking.total_amount) as total_amount
        from booking
        join inserted
        on booking.booking_id = inserted.booking_id
        group by booking.invoice_id
    ) i on inv.invoice_id = i.invoice_id
end
go

--update booking total amount--
create trigger UpdateAmountBooking on booking
for update
as
begin
    -- Update booking
    update b
    set b.total_amount = rt.room_price * datediff(day, i.check_in_date, i.check_out_date)
    from booking b
    join inserted i on b.booking_id = i.booking_id and b.room_id = i.room_id
    join room r on r.room_id = i.room_id
    join room_type rt on rt.room_type_id = r.room_type_id
    join deleted d on d.booking_id = i.booking_id
    where d.check_out_date <= i.check_out_date and
              i.check_in_date <= d.check_in_date

    -- update invoice
    update inv
    set inv.total_amount = inv.total_amount + i.total_amount_1 + total_amount_2
    from invoice inv
    join (
        select inserted.invoice_id,
               sum(rt.room_price * datediff(day, d.check_out_date, inserted.check_out_date)) as total_amount_1,
               sum(rt.room_price * datediff(day, inserted.check_in_date, d.check_in_date)) as total_amount_2
        from inserted
        join deleted d on d.booking_id = inserted.booking_id
        join room r on r.room_id = inserted.room_id
        join room_type rt on rt.room_type_id = r.room_type_id
        where d.check_out_date <= inserted.check_out_date and
              inserted.check_in_date <= d.check_in_date
        group by inserted.invoice_id
    ) i on inv.invoice_id = i.invoice_id
end
go

--// insert data //--

--staff--
insert into staff (staff_id, full_name, position, contact_number, email, address, birthday, gender, salary)
values
('ST001', 'John Doe', 'Hotel Manager', '1234567890', 'john.doe@example.com', '123 Street, City', '01-01-1980', 'male', 5000),
('ST002', 'Jane Doe', 'Front Desk Clerk', '1234567891', 'jane.doe@example.com', '124 Street, City', '02-02-1981', 'female', 4000),
('ST003', 'Jim Brown', 'Front Desk Clerk', '1234567892', 'jim.brown@example.com', '125 Street, City', '03-03-1982', 'male', 3000),
('ST004', 'Jill Smith', 'Front Desk Clerk', '1234567893', 'jill.smith@example.com', '126 Street, City', '04-04-1983', 'female', 5000),
('ST005', 'Joe Davis', 'Front Desk Clerk', '1234567894', 'joe.davis@example.com', '127 Street, City', '05-05-1984', 'male', 4000),
('ST006', 'Julia Johnson', 'Concierge', '1234567895', 'julia.johnson@example.com', '128 Street, City', '06-06-1985', 'female', 3000),
('ST007', 'Jerry Martinez', 'Concierge', '1234567896', 'jerry.martinez@example.com', '129 Street, City', '07-07-1986', 'male', 5000),
('ST008', 'Jessica Robinson', 'Chef', '1234567897', 'jessica.robinson@example.com', '130 Street, City', '08-08-1987', 'female', 4000),
('ST009', 'Jack Harris', 'Porter', '1234567898', 'jack.harris@example.com', '131 Street, City', '09-09-1988', 'male', 3000),
('ST010', 'Jasmine Clark', 'Porter', '1234567899', 'jasmine.clark@example.com', '132 Street, City', '10-10-1989', 'female', 5000),
('ST011', 'Jacob Lewis', 'Maintenance', '1234567800', 'jacob.lewis@example.com', '133 Street, City', '11-11-1990', 'male', 4000),
('ST012', 'Julie Lee', 'Maintenance', '1234567801', 'julie.lee@example.com', '134 Street, City', '12-12-1991', 'female', 3000),
('ST013', 'Jeff Walker', 'Maintenance', '1234567802', 'jeff.walker@example.com', '135 Street, City', '13-01-1992', 'male', 5000),
('ST014', 'Jenny Hall', 'Security', '1234567803', 'jenny.hall@example.com', '136 Street, City', '14-02-1993', 'female', 4000),
('ST015', 'James Allen', 'Security', '1234567804', 'james.allen@example.com', '137 Street, City', '15-03-1994', 'male', 3000),
('ST016', 'Judy Young', 'Security', '1234567805', 'judy.young@example.com', '138 Street, City', '16-04-1995', 'female', 5000)

--account--
insert into account (username, password, profile_picture, staff_id)
values
('johndoe123', '1234', null, 'ST001'),
('janedoe123', '2345', null, 'ST002'),
('jimbrown123', '3456', null, 'ST003'),
('jillsmith123', '4567', null, 'ST004'),
('joedavis123', '5678', null, 'ST005')

--customer--
insert into customer (customer_id, full_name, contact_number, email, address, gender, credit_card, id_proof)
values
('C0001', 'John Doe', '1234567890', 'john.doe@example.com', '123 Street, City', 'male', '1111-2222-3333-4444', 'ID00001'),
('C0002', 'Jane Doe', '1234567891', 'jane.doe@example.com', '124 Street, City', 'female', '2222-3333-4444-5555', 'ID00002'),
('C0003', 'Jim Brown', '1234567892', 'jim.brown@example.com', '125 Street, City', 'male', '3333-4444-5555-6666', 'ID00003'),
('C0004', 'Jill Smith', '1234567893', 'jill.smith@example.com', '126 Street, City', 'female', '4444-5555-6666-7777', 'ID00004'),
('C0005', 'Joe Davis', '1234567894', 'joe.davis@example.com', '127 Street, City', 'male', '5555-6666-7777-8888', 'ID00005'),
('C0006', 'Julia Johnson', '1234567895', 'julia.johnson@example.com', '128 Street, City', 'female', '6666-7777-8888-9999', 'ID00006'),
('C0007', 'Jerry Martinez', '1234567896', 'jerry.martinez@example.com', '129 Street, City', 'male', '7777-8888-9999-0000', 'ID00007'),
('C0008', 'Jessica Robinson', '1234567897', 'jessica.robinson@example.com', '130 Street, City', 'female', '8888-9999-0000-1111', 'ID00008'),
('C0009', 'Jack Harris', '1234567898', 'jack.harris@example.com', '131 Street, City', 'male', '9999-0000-1111-2222', 'ID00009'),
('C0010', 'Jasmine Clark', '1234567899', 'jasmine.clark@example.com', '132 Street, City', 'female', '0000-1111-2222-3333', 'ID00010');

--room_type--
insert into room_type (room_type_id, room_type_name, capacity, bed_amount, room_price, room_type_desc)
values
('RT001', 'Single', 1, 1, 100.00, 'A room assigned to one person'),
('RT002', 'Double', 2, 1, 200.00, 'A room assigned to one person'),
('RT003', 'Twin', 2, 2, 300.00, 'A room with two twin beds'),
('RT004', 'Double-double', 4, 2, 400.00, 'A Room with two double beds')

--room--
insert into room (room_id, room_number, notes, room_type_id)
values
('R0001', '101', null, 'RT001'),
('R0002', '102', null, 'RT002'),
('R0003', '103', null, 'RT003'),
('R0004', '104', null, 'RT004'),
('R0005', '105', null, 'RT001'),
('R0006', '201', null, 'RT002'),
('R0007', '202', null, 'RT003'),
('R0008', '203', 'Air conditioner is broken', 'RT004'),
('R0009', '204', null, 'RT001'),
('R0010', '205', null, 'RT002'),
('R0011', '301', null, 'RT003'),
('R0012', '302', null, 'RT004'),
('R0013', '303', null, 'RT001'),
('R0014', '304', 'Light is broken', 'RT002'),
('R0015', '305', null, 'RT003'),
('R0016', '401', null, 'RT004'),
('R0017', '402', null, 'RT001'),
('R0018', '403', null, 'RT002'),
('R0019', '404', null, 'RT003'),
('R0020', '405', null, 'RT004');

--service--
insert into service (service_id, service_name, service_type, service_price)
values
('S0001', 'Coffee', 'Drink', 5.00),
('S0002', 'Tea', 'Drink', 4.00),
('S0003', 'Juice', 'Drink', 6.00),
('S0004', 'Burger', 'Food', 10.00),
('S0005', 'Pizza', 'Food', 15.00),
('S0006', 'Pasta', 'Food', 12.00),
('S0007', 'Room Cleaning', 'Service', 20.00),
('S0008', 'Laundry', 'Service', 15.00),
('S0009', 'Spa', 'Service', 50.00),
('S0010', 'Gym Access', 'Service', 30.00),
('S0011', 'Breakfast', 'Food', 10.00),
('S0012', 'Lunch', 'Food', 15.00),
('S0013', 'Dinner', 'Food', 20.00),
('S0014', 'Cocktail', 'Drink', 8.00),
('S0015', 'Beer', 'Drink', 6.00),
('S0016', 'Wine', 'Drink', 10.00),
('S0017', 'Tour Guide', 'Service', 80.00)

--invoice--
insert into invoice (invoice_id, customer_id, staff_id, invoice_date, total_amount, payment_type)
values
('I0001', 'C0001', 'ST002', '01-08-2023', 0, 'Cash'),
('I0002', 'C0002', 'ST003', '02-08-2023', 0, 'Cash'),
('I0003', 'C0003', 'ST004', '15-08-2023', 0, 'Credit card'),
('I0004', 'C0004', 'ST005', '12-09-2023', 0, 'Credit card'),
('I0005', 'C0005', 'ST002', '14-09-2023', 0, 'Cash'),
('I0006', 'C0006', 'ST003', '23-09-2023', 0, 'Cash'),
('I0007', 'C0007', 'ST004', '02-10-2023', 0, 'Credit card'),
('I0008', 'C0008', 'ST005', '05-10-2023', 0, 'Credit card'),
('I0009', 'C0009', 'ST002', '11-10-2023', 0, 'Cash'),
('I0010', 'C0010', 'ST003', '22-10-2023', 0, 'Credit card')

--booking--
insert into booking (booking_id, invoice_id, room_id, guest_quantity, check_in_date, check_out_date, total_amount)
values
('B0021', 'I0001', 'R0001', 1, '26-12-2023', '30-12-2023', null),
('B0022', 'I0001', 'R0001', 1, '24-12-2023', '30-12-2023', null),
('B0001', 'I0001', 'R0001', 1, '01-08-2023', '07-08-2023', null),
('B0002', 'I0001', 'R0002', 2, '01-08-2023', '07-08-2023', null),
('B0003', 'I0002', 'R0003', 2, '02-08-2023', '05-08-2023', null),
('B0004', 'I0003', 'R0004', 4, '20-08-2023', '24-08-2023', null),
('B0005', 'I0003', 'R0006', 2, '20-08-2023', '24-08-2023', null),
('B0006', 'I0004', 'R0012', 4, '15-09-2023', '20-09-2023', null),
('B0007', 'I0004', 'R0016', 4, '15-09-2023', '20-09-2023', null),
('B0008', 'I0004', 'R0020', 4, '15-09-2023', '20-09-2023', null),
('B0009', 'I0005', 'R0009', 1, '14-09-2023', '17-09-2023', null),
('B0010', 'I0005', 'R0010', 2, '14-09-2023', '17-09-2023', null),
('B0011', 'I0006', 'R0011', 2, '23-09-2023', '25-09-2023', null),
('B0012', 'I0007', 'R0005', 1, '02-01-2024', '04-01-2024', null),
('B0013', 'I0007', 'R0013', 1, '02-01-2024', '04-01-2024', null),
('B0014', 'I0008', 'R0002', 2, '05-01-2024', '09-01-2024', null),
('B0015', 'I0008', 'R0003', 2, '05-01-2024', '09-01-2024', null),
('B0016', 'I0009', 'R0004', 4, '11-01-2024', '14-01-2024', null),
('B0017', 'I0009', 'R0012', 4, '11-01-2024', '14-01-2024', null),
('B0018', 'I0009', 'R0018', 2, '11-01-2024', '14-01-2024', null),
('B0019', 'I0010', 'R0019', 2, '23-01-2024', '25-01-2024', null),
('B0020', 'I0010', 'R0020', 4, '23-01-2024', '25-01-2024', null);

--service_use--
insert into service_use (invoice_id, service_id, service_quantity, total_amount)
values
('I0001', 'S0001', 1, null),
('I0001', 'S0004', 2, null),
('I0002', 'S0008', 1, null),
('I0002', 'S0013', 1, null),
('I0003', 'S0015', 2, null),
('I0004', 'S0010', 1, null),
('I0004', 'S0012', 2, null),
('I0005', 'S0014', 3, null),
('I0005', 'S0017', 1, null),
('I0005', 'S0009', 1, null)

