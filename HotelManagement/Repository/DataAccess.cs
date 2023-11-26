// using HotelManagement.Model;
//
// namespace HotelManagement.Repository;
//
// public class DataAccess
// {
//     public List<Staff> GetStaff()
//     {
//         using var context = new HotelManagementContext();
//         var staffModel = context.Staff;
//         var query =
//             from staff in staffModel
//             select new
//             {
//                 staff.StaffId,
//                 staff.FullName,
//                 staff.Position,
//                 staff.ContactNumber,
//                 staff.Email,
//                 staff.Address,
//                 staff.Birthday,
//                 staff.Gender,
//                 staff.Salary
//             };
//         List<Staff> queryList = query.ToList();
//         
//     }
// }
//
//
