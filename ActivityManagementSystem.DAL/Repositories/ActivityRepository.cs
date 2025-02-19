using ActivityManagementSystem.DAL.Constants;
using ActivityManagementSystem.DAL.Infrastructure;
using ActivityManagementSystem.DAL.Interfaces;
using ActivityManagementSystem.Domain.AppSettings;
using ActivityManagementSystem.Domain.Models.Activity;
using ClosedXML.Excel;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Drawing;
using RestSharp;
using System.Web;
using DocumentFormat.OpenXml.Office2010.Excel;
using Table = Xceed.Document.NET.Table;
using Color = System.Drawing.Color;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
//using Microsoft.Office.Interop.Word;
//using Aspose.Words;

namespace ActivityManagementSystem.DAL.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        // public WrappingVerticalAlignment VerticalAlignment { get; set; }
        private readonly AppSettings _appSettings;

        private readonly IDataBaseConnection _db;
        //private readonly ILogger<ActivityRepository> _logger;


        public ActivityRepository(AppSettings appSettings, IDataBaseConnection db)
        {
            this._appSettings = appSettings;
            this._db = db;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Connection?.Dispose();
            }
        }

        public Task<List<ActivityModel>> GetAllActivityData(int Type, long? DepartmentId)
        {
            var spName = ConstantSPnames.SP_GETAllACTIVITYDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ActivityModel>(spName,
                new { Id = Type, DepartmentID = DepartmentId }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<ActivityModel>> GetActivityData(int? id)
        {
            var spName = ConstantSPnames.SP_GETACTIVITYDETAILS;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<ActivityModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }
        public Task<List<FacultyDropdown>> GetFacultyByName(string facultyName)
        {
            var spName = ConstantSPnames.SP_GETFACULTYBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<FacultyDropdown>(spName, new
            {
                FacultyName = facultyName


            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ActivityModel>> InsertActivityData(ActivityModel activityData)
        {
            var spName = ConstantSPnames.SP_INSERTACTIVITYDETAILS;
            List<string> lst = new List<string>();
            if (activityData.FilePath != "" && activityData.FilePath != null)
            {
                string[] filePaths = Directory.GetFiles(activityData.FilePath);
                foreach (var file in filePaths)
                {
                    lst.Add(Path.GetFileName(file));
                    Console.WriteLine(file);
                }

                activityData.Files = lst;
            }

            return Task.Factory.StartNew(() => _db.Connection.Query<ActivityModel>(spName, new
            {
                ActivityID = activityData.ActivityID,

                Data = activityData.Data,
                DepartmentID = activityData.DepartmentID,
                //FilePath = activityData.FilePath,
                CreatedBy = activityData.CreatedBy,
                CreatedDate = activityData.CreatedDate
                // Files = activityData.Files

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<ActivityModel>> UpdateActivityData(ActivityModel activityData)
        {
            var spName = ConstantSPnames.SP_UPDATEACTIVITYDETAILS;


            return Task.Factory.StartNew(() => _db.Connection.Query<ActivityModel>(spName, new
            {
                Id = activityData.Id,
                ActivityID = activityData.ActivityID,

                Data = activityData.Data,
                DepartmentID = activityData.DepartmentID,
                //FilePath = activityData.FilePath,
                //Files = activityData.Files,
                ModifiedBy = activityData.ModifiedBy,
                ModifiedDate = activityData.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public Task<List<ActivityModel>> DeleteActivityData(int id)
        {
            var spName = ConstantSPnames.SP_DELETEACTIVITYDETAILS;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<ActivityModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }



      

       



        public Task<List<RoleModel>> GetRole(int? id)
        {
            var spName = ConstantSPnames.SP_GETROLEMASTER;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<RoleModel>(spName, new { RoleId = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public Task<List<RoleActivity>> GetRoleActivity(int? id)
        {
            var spName = ConstantSPnames.SP_GETROLEACTIVITY;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<RoleActivity>(spName, new { @Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public Task<List<RoleModel>> InsertRole(RoleModel rolemaster)
        {
            var spName = ConstantSPnames.SP_INSERTROLEMASTER;
            return Task.Factory.StartNew(() => _db.Connection.Query<RoleModel>(spName, new
            {
                Name = rolemaster.Name,
                // RoleId=rolemaster.RoleId  ,
                CreatedBy = rolemaster.CreatedBy,
                CreatedDate = rolemaster.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<RoleModel>> UpdateRole(RoleModel rolemaster)
        {
            var spName = ConstantSPnames.SP_UPDATEROLEMASTER;
            return Task.Factory.StartNew(() => _db.Connection.Query<RoleModel>(spName, new
            {
                Name = rolemaster.Name,
                RoleId = rolemaster.RoleId,
                ModifiedBy = rolemaster.ModifiedBy,
                ModifiedDate = rolemaster.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public Task<List<RoleActivity>> UpdateRoleActivity(RoleActivity roleactivity)
        {
            var spName = ConstantSPnames.SP_UPDATEROLEACTIVITY;
            return Task.Factory.StartNew(() => _db.Connection.Query<RoleActivity>(spName, new
            {

                Id = roleactivity.Id,
                RoleId = roleactivity.RoleId,
                PaperPresentation = roleactivity.PaperPresentation,
                ProjectOrModel = roleactivity.ProjectOrModel,
                GuestLectures = roleactivity.GuestLectures,
                ImplantTrainingOrInternship = roleactivity.ImplantTrainingOrInternship,
                IndustrialVisits = roleactivity.IndustrialVisits,
                SportsAndGames = roleactivity.SportsAndGames,
                NCC = roleactivity.NCC,
                NSS = roleactivity.NSS,
                FacultyDevelopment = roleactivity.FacultyDevelopment,
                WomenInDevelopment = roleactivity.WomenInDevelopment,
                JournalOrBookpublication = roleactivity.JournalOrBookpublication,
                PatentDetails = roleactivity.PatentDetails,
                SemesterResult = roleactivity.SemesterResult,
                SymposiumAndExpo = roleactivity.SymposiumAndExpo,
                Placement = roleactivity.Placement,
                ExtensionServices = roleactivity.ExtensionServices,
                Grants = roleactivity.Grants,
                AlumniEvent = roleactivity.AlumniEvent,
                Consultancy = roleactivity.Consultancy,
                Awards = roleactivity.Awards,
                Events = roleactivity.Events,
                MOUs = roleactivity.MOUs,
                Advertisements = roleactivity.Advertisements,
                PressReports = roleactivity.PressReports,
                Miscellaneous = roleactivity.Miscellaneous,
                UpcomingEvents = roleactivity.UpcomingEvents,
                ModifiedBy = roleactivity.ModifiedBy,
                ModifiedDate = roleactivity.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public Task<List<RoleActivity>> InsertRoleActivity(RoleActivity roleactivity)
        {
            var spName = ConstantSPnames.SP_INSERTROLEACTIVITY;
            return Task.Factory.StartNew(() => _db.Connection.Query<RoleActivity>(spName, new
            {
                RoleId = roleactivity.RoleId,
                PaperPresentation = roleactivity.PaperPresentation,
                ProjectOrModel = roleactivity.ProjectOrModel,
                GuestLectures = roleactivity.GuestLectures,
                ImplantTrainingOrInternship = roleactivity.ImplantTrainingOrInternship,
                IndustrialVisits = roleactivity.IndustrialVisits,
                SportsAndGames = roleactivity.SportsAndGames,
                NCC = roleactivity.NCC,
                NSS = roleactivity.NSS,
                FacultyDevelopment = roleactivity.FacultyDevelopment,
                WomenInDevelopment = roleactivity.WomenInDevelopment,
                JournalOrBookpublication = roleactivity.JournalOrBookpublication,
                PatentDetails = roleactivity.PatentDetails,
                SemesterResult = roleactivity.SemesterResult,
                SymposiumAndExpo = roleactivity.SymposiumAndExpo,
                Placement = roleactivity.Placement,
                ExtensionServices = roleactivity.ExtensionServices,
                Grants = roleactivity.Grants,
                AlumniEvent = roleactivity.AlumniEvent,
                Consultancy = roleactivity.Consultancy,
                Awards = roleactivity.Awards,
                Events = roleactivity.Events,
                MOUs = roleactivity.MOUs,
                Advertisements = roleactivity.Advertisements,
                PressReports = roleactivity.PressReports,
                Miscellaneous = roleactivity.Miscellaneous,
                UpcomingEvents = roleactivity.UpcomingEvents,

                CreatedBy = roleactivity.CreatedBy,
                CreatedDate = roleactivity.CreatedDate,
                ModifiedBy = roleactivity.ModifiedBy,
                ModifiedDate = roleactivity.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public string DeleteRole(int id)
        {
            var spName = ConstantSPnames.SP_DELETEROLEMASTER;
            try
            {
                using (SqlConnection sqlconnection =
                       new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    sqlconnection.Open();


                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("roleId", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                    return "Success";
                }

                //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            // return Task.Factory.StartNew(() => _db.Connection.Query<RoleModel>(spName, new { RoleId = id }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<StudentModel>> GetStudentDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETSTUDENT;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<StudentModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public Task<List<UserModel>> GetUserDetails(string Username, string Password,string role)
        {
            var spName = ConstantSPnames.SP_GETUSERDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<UserModel>(spName, new
            {
                UserName = Username,
                Password = Password,
                Role =role
            }, commandType: CommandType.StoredProcedure).ToList());
        }

    
        public Task<List<HouseModel>> GetAllHouse(int? Id)
        {
            var spName = ConstantSPnames.SP_GETALLHOUSE;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseModel>(spName, new
            {
                Id = Id
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<AlumniDropdown>> GetAlumniByName(string AlumniName)
        {
            var spName = ConstantSPnames.SP_GETAlumniBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<AlumniDropdown>(spName, new
            {
                AlumniName = AlumniName
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<StudentDropdown>> GetStudentByName(string StudentyName)
        {
            var spName = ConstantSPnames.SP_GETSTUDENTBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentDropdown>(spName, new
            {
                StudentName = StudentyName

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<StudentModel>> InsertStudentDetails(StudentModel studentDetails)
        {
            var spName = ConstantSPnames.SP_INSERTSTUDENT;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentModel>(spName, new
            {
                AdmissionNumber = studentDetails.AdmissionNumber,
                Student_FirstName = studentDetails.Student_FirstName,
                Student_MiddleName = studentDetails.Student_MiddleName,
                Student_LastName = studentDetails.Student_LastName,
                AllergicTo =studentDetails.AllergicTo,
                HouseId=studentDetails.HouseId,
                BloodGroup=studentDetails.BloodGroup,
                Gender = studentDetails.Gender,
                Dob = studentDetails.Dob,
                
                CommunicationAddress = studentDetails.CommunicationAddress,
                PermanentAddress = studentDetails.PermanentAddress,
                Student_AadhaarNumber = studentDetails.Student_AadhaarNumber,
                ParentEmailId = studentDetails.ParentEmailId,
                Photo = studentDetails.Photo,               
                FatherName = studentDetails.FatherName,
                Father_MobileNumber = studentDetails.Father_MobileNumber,
                Father_Photo = studentDetails.Father_Photo,
                MotherName = studentDetails.MotherName,
                Mother_MobileNumber = studentDetails.Mother_MobileNumber,
                Mother_Photo = studentDetails.Mother_Photo,
                Gardian1Name =studentDetails.Gardian1Name,
                Gardian1MobileNumber =studentDetails.Gardian1MobileNumber,
                Gardian1Photo =studentDetails.Gardian1Photo,
                Gardian2Name = studentDetails.Gardian2Name,
                Gardian2MobileNumber = studentDetails.Gardian2MobileNumber,
                Gardian2Photo = studentDetails.Gardian2Photo,
               
               
                CreatedBy = studentDetails.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public Task<List<HouseModel>> InsertHouseDetails(HouseModel house)
        {
            var spName = ConstantSPnames.SP_INSERTHOUSE;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseModel>(spName, new
            {
                Name=house.Name,
                ISActive = house.IsActive,
                CreatedBy = house.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<StudentModel>> UpdateStudentDetails(StudentModel studentDetails)
        {
            var spName = ConstantSPnames.SP_UPDATESTUDENT;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentModel>(spName, new
            {
                Id = studentDetails.Id,
                AdmissionNumber = studentDetails.AdmissionNumber,
                Student_FirstName = studentDetails.Student_FirstName,
                Student_MiddleName = studentDetails.Student_MiddleName,
                Student_LastName = studentDetails.Student_LastName,

               
                AllergicTo = studentDetails.AllergicTo,
                HouseId = studentDetails.HouseId,
                BloodGroup = studentDetails.BloodGroup,
                Gender = studentDetails.Gender,
                Dob = studentDetails.Dob,

                CommunicationAddress = studentDetails.CommunicationAddress,
                PermanentAddress = studentDetails.PermanentAddress,
                Student_AadhaarNumber = studentDetails.Student_AadhaarNumber,
                ParentEmailId = studentDetails.ParentEmailId,
                Photo = studentDetails.Photo,
                FatherName = studentDetails.FatherName,
                Father_MobileNumber = studentDetails.Father_MobileNumber,
                Father_Photo = studentDetails.Father_Photo,
                motherName = studentDetails.MotherName,
                Mother_MobileNumber = studentDetails.Mother_MobileNumber,
                Mother_Photo = studentDetails.Mother_Photo,
                Gardian1Name = studentDetails.Gardian1Name,
                Gardian1MobileNumber = studentDetails.Gardian1MobileNumber,
                Gardian1Photo = studentDetails.Gardian1Photo,
                Gardian2Name = studentDetails.Gardian2Name,
                Gardian2MobileNumber = studentDetails.Gardian2MobileNumber,
                Gardian2Photo = studentDetails.Gardian2Photo,
               
                ModifiedBy = studentDetails.ModifiedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<StudentModel>> DeleteStudentDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETESTUDENT;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<StudentModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public Task<List<FacultyModel>> GetFacultyDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETFACULTY;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<FacultyModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public string InsertFacultyDetails(FacultyModel facultyDetails)
        {
            var spName = ConstantSPnames.SP_INSERTFACULTY;
            using (SqlConnection sqlconnection =
                     new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                try
                {
                    sqlconnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;
                    // command.Parameters.Add("Id", SqlDbType.Int).Value = facultyDetails.Id;
                    command.Parameters.Add("UserName", SqlDbType.VarChar).Value = facultyDetails.UserName;
                    command.Parameters.Add("Password", SqlDbType.VarChar).Value = facultyDetails.Password;
                    command.Parameters.Add("RoleId", SqlDbType.BigInt).Value = facultyDetails.RoleId;
                    command.Parameters.Add("FacultyId", SqlDbType.VarChar).Value = facultyDetails.FacultyId;
                    command.Parameters.Add("Faculty_FirstName", SqlDbType.VarChar).Value = facultyDetails.Faculty_FirstName;
                    command.Parameters.Add("Faculty_MiddleName", SqlDbType.VarChar).Value = facultyDetails.Faculty_MiddleName;
                    command.Parameters.Add("Faculty_LastName", SqlDbType.VarChar).Value = facultyDetails.Faculty_LastName;
                    command.Parameters.Add("Gender", SqlDbType.VarChar).Value = facultyDetails.Gender;
                    command.Parameters.Add("DOB", SqlDbType.DateTime).Value = facultyDetails.DOB;
                    command.Parameters.Add("FacultyMobileNo_1", SqlDbType.VarChar).Value = facultyDetails.FacultyMobileNo_1;
                    command.Parameters.Add("FacultyMobileNo_2", SqlDbType.VarChar).Value = facultyDetails.FacultyMobileNo_2;
                    command.Parameters.Add("Email", SqlDbType.VarChar).Value = facultyDetails.Email;
                    command.Parameters.Add("FilePath", SqlDbType.VarChar).Value = facultyDetails.FilePath;
                    command.Parameters.Add("FileNames", SqlDbType.VarChar).Value = facultyDetails.FileNames;
                    command.Parameters.Add("BloodGroup", SqlDbType.VarChar).Value = facultyDetails.BloodGroup;
                    command.Parameters.Add("Address", SqlDbType.VarChar).Value = facultyDetails.Address;
                    command.Parameters.Add("CreatedBy", SqlDbType.VarChar).Value = facultyDetails.CreatedBy;
                    command.Parameters.Add("CreatedDate", SqlDbType.DateTime).Value = facultyDetails.CreatedDate;
                    command.Parameters.Add("ModifiedBy", SqlDbType.VarChar).Value = facultyDetails.ModifiedBy;
                    command.Parameters.Add("ModifiedDate", SqlDbType.DateTime).Value = facultyDetails.ModifiedDate;
                    int result = command.ExecuteNonQuery();
                    sqlconnection.Close();
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

                //return Task.Factory.StartNew(() => _db.Connection.Query<FacultyModel>(spName, new
                //{
                //    UserName = facultyDetails.UserName,
                //    Password = facultyDetails.Password,
                //    RoleId = facultyDetails.RoleId,
                //    FacultyId = facultyDetails.FacultyId,
                //    Faculty_FirstName = facultyDetails.Faculty_FirstName,
                //    Faculty_MiddleName = facultyDetails.Faculty_MiddleName,
                //    Faculty_LastName = facultyDetails.Faculty_LastName,
                //    Gender = facultyDetails.Gender,
                //    IndentRoleName = facultyDetails.IndentRoleName,
                //    FdpRoleName = facultyDetails.FdpRoleName,
                //    DepartmentId = facultyDetails.DepartmentId,
                //    FacultyMobileNo_1 = facultyDetails.FacultyMobileNo_1,
                //    FacultyMobileNo_2 = facultyDetails.FacultyMobileNo_2,
                //    Email = facultyDetails.Email,
                //    Photo = facultyDetails.Photo,
                //    CreatedBy = facultyDetails.CreatedBy,
                //    CreatedDate = facultyDetails.CreatedDate,
                //    ModifiedBy = facultyDetails.ModifiedBy,
                //    ModifiedDate = facultyDetails.ModifiedDate
                //}, commandType: CommandType.StoredProcedure).ToList());
            }
        }
        public string UpdateFacultyDetails(FacultyModel facultyDetails)
        {
            var spName = ConstantSPnames.SP_UPDATEFACULTY;

            using (SqlConnection sqlconnection =
                  new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                try
                {
                    sqlconnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Id", SqlDbType.Int).Value = facultyDetails.Id;
                    command.Parameters.Add("UserName", SqlDbType.VarChar).Value = facultyDetails.UserName;
                    command.Parameters.Add("Password", SqlDbType.VarChar).Value = facultyDetails.Password;
                    command.Parameters.Add("RoleId", SqlDbType.BigInt).Value = facultyDetails.RoleId;
                    command.Parameters.Add("FacultyId", SqlDbType.VarChar).Value = facultyDetails.FacultyId;
                    command.Parameters.Add("Faculty_FirstName", SqlDbType.VarChar).Value = facultyDetails.Faculty_FirstName;
                    command.Parameters.Add("Faculty_MiddleName", SqlDbType.VarChar).Value = facultyDetails.Faculty_MiddleName ?? (object)DBNull.Value;
                    command.Parameters.Add("Faculty_LastName", SqlDbType.VarChar).Value = facultyDetails.Faculty_LastName;
                    command.Parameters.Add("Gender", SqlDbType.VarChar).Value = facultyDetails.Gender;
                    command.Parameters.Add("DOB", SqlDbType.DateTime).Value = facultyDetails.DOB;
                  
                    command.Parameters.Add("FacultyMobileNo_1", SqlDbType.VarChar).Value = facultyDetails.FacultyMobileNo_1;
                    command.Parameters.Add("FacultyMobileNo_2", SqlDbType.VarChar).Value = facultyDetails.FacultyMobileNo_2 ?? (object)DBNull.Value;
                    command.Parameters.Add("Email", SqlDbType.VarChar).Value = facultyDetails.Email;
                    command.Parameters.Add("FilePath", SqlDbType.VarChar).Value = facultyDetails.FilePath;
                    command.Parameters.Add("FileNames", SqlDbType.VarChar).Value = facultyDetails.FileNames;
                    command.Parameters.Add("BloodGroup", SqlDbType.VarChar).Value = facultyDetails.BloodGroup;
                    command.Parameters.Add("Address", SqlDbType.VarChar).Value = facultyDetails.Address;
                    command.Parameters.Add("CreatedBy", SqlDbType.VarChar).Value = facultyDetails.CreatedBy;
                    command.Parameters.Add("CreatedDate", SqlDbType.DateTime).Value = facultyDetails.CreatedDate;
                    command.Parameters.Add("ModifiedBy", SqlDbType.VarChar).Value = facultyDetails.ModifiedBy;
                    command.Parameters.Add("ModifiedDate", SqlDbType.DateTime).Value = facultyDetails.ModifiedDate;
                    int result = command.ExecuteNonQuery();
                    sqlconnection.Close();
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
                //return Task.Factory.StartNew(() => _db.Connection.Query<string>(spName, new
                //{
                //    Id = facultyDetails.Id,
                //    UserName = facultyDetails.UserName,
                //    Password = facultyDetails.Password,
                //    RoleId = facultyDetails.RoleId,
                //    FacultyId = facultyDetails.FacultyId,
                //    Faculty_FirstName = facultyDetails.Faculty_FirstName,
                //    Faculty_MiddleName = facultyDetails.Faculty_MiddleName,
                //    Faculty_LastName = facultyDetails.Faculty_LastName,
                //    Gender = facultyDetails.Gender,
                //    DepartmentId = facultyDetails.DepartmentId,
                //    IndentRoleName = facultyDetails.IndentRoleName,
                //    FdpRoleName = facultyDetails.FdpRoleName,
                //    FacultyMobileNo_1 = facultyDetails.FacultyMobileNo_1,
                //    FacultyMobileNo_2 = facultyDetails.FacultyMobileNo_2,
                //    Email = facultyDetails.Email,
                //    Photo = facultyDetails.Photo,
                //    CreatedBy = facultyDetails.CreatedBy,
                //    CreatedDate = facultyDetails.CreatedDate,
                //    ModifiedBy = facultyDetails.ModifiedBy,
                //    ModifiedDate = facultyDetails.ModifiedDate
                //}, commandType: CommandType.StoredProcedure).ToString());
            }

        }

        public Task<List<HouseModel>> UpdateHouseDetails(HouseModel house)
        {
            var spName = ConstantSPnames.SP_UPDATEHOUSE;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseModel>(spName, new
            {
                Id = house.Id,
               Name=house.Name,
                ISActive = house.IsActive,
                ModifiedBy = house.ModifiedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<HouseModel>> DeleteHouseDetails(int id)
        {
            
                var spName = ConstantSPnames.SP_DELETEHOUSE;

                return Task.Factory.StartNew(() =>
                _db.Connection.Query<HouseModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
                //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
          


        }

        public Task<List<FacultyModel>> DeleteFacultyDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEFACULTY;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<FacultyModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());

        }

        public Task<string> UpdateActivityFilepathdata(string target, int id, string filesList)
        {
            var spName = ConstantSPnames.SP_UPDATEACTIVITYFILE;
            //  var list = new List<string>;

            return Task.Factory.StartNew(() => _db.Connection.Query<string>(spName,
                    new { Id = id, filepath = target, files = filesList }, commandType: CommandType.StoredProcedure)
                .ToString());
        }

        public Task<string> UpdateFilepathdata(string target, int id, string filesList, string TypeofFile)
        {
            var spName = ConstantSPnames.SP_UPDATEFILE;
            //  var list = new List<string>;

            return Task.Factory.StartNew(() => _db.Connection.Query<string>(spName,
                new { Id = id, filepath = target, files = filesList, typeofFile = TypeofFile },
                commandType: CommandType.StoredProcedure).ToString());
        }

        //public Task<List<DepartmentModel>> GetAllDepartment(int? id)
        //{
        //    var spName = ConstantSPnames.GETHOUSE;
        //    return Task.Factory.StartNew(()=> _db.Connection.Query<DepartmentModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        //}
        public Task<List<SubjectModel>> GetAllSubject(int? Id)
        {
            var spName = ConstantSPnames.SP_GETALLSUBJECT;
            return Task.Factory.StartNew(() => _db.Connection.Query<SubjectModel>(spName, new
            {
                Id = Id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<SubjectModel>> InsertSubjectDetails(SubjectModel subject)
        {
            var spName = ConstantSPnames.SP_INSERTSUBJECT;
            return Task.Factory.StartNew(() => _db.Connection.Query<SubjectModel>(spName, new
            {
                SubjectShortForm = subject.SubjectShortForm,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                SubSem = subject.SubSem,
                SubYear = subject.SubYear,
                CreatedBy = subject.CreatedBy,
                CreatedDate = subject.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<SubjectModel>> UpdateSubjectDetails(SubjectModel subject)
        {
            var spName = ConstantSPnames.SP_UPDATESUBJECT;
            return Task.Factory.StartNew(() => _db.Connection.Query<SubjectModel>(spName, new
            {
                Id = subject.Id,
                SubjectShortForm = subject.SubjectShortForm,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                SubSem = subject.SubSem,
                SubYear = subject.SubYear,
                ModifiedBy = subject.ModifiedBy,
                ModifiedDate = subject.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public string DeleteSubjectDetails(int id)
        {
            try
            {
                var spName = ConstantSPnames.SP_DELETESUBJECT;
                using (SqlConnection sqlconnection =
                       new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    sqlconnection.Open();


                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("Id", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                    return "Success";
                }

                //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            //return Task.Factory.StartNew(() => _db.Connection.Query<SubjectModel>(spName, new { id = id }, commandType: CommandType.StoredProcedure).ToList());

        }

        public Task<List<SectionModel>> GetAllSectiones(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLSECTION;
            return Task.Factory.StartNew(() => _db.Connection.Query<SectionModel>(spName, new
            {
                Id = id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<SectionModel>> InsertSectionDetails(SectionModel section)
        {
            var spName = ConstantSPnames.SP_INSERTSECTION;
            return Task.Factory.StartNew(() => _db.Connection.Query<SectionModel>(spName, new
            {
                GradeOrClass=section.GradeOrClass,
                Section =section.Section,
                CoordinatorId=section.CoordinatorId,
                Teachers =section.Teachers,
                IsActive=section.IsActive,
                CreatedBy = section.CreatedBy
                

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<SectionModel>> UpdateSectionDetails(SectionModel section)
        {
            var spName = ConstantSPnames.SP_UPDATESECTION;
            return Task.Factory.StartNew(() => _db.Connection.Query<SectionModel>(spName, new
            {
                Id=section.Id,
                GradeOrClass = section.GradeOrClass,
                Section = section.Section,
                CoordinatorId = section.CoordinatorId,
                Teachers = section.Teachers,
                IsActive = section.IsActive,

                ModifiedBy = section.ModifiedBy

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        public Task<List<SectionModel>> DeleteSectionDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETESECTION;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<SectionModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());

        }

        public Task<List<BatchStudMappingModel>> GetAllBatchStudMappings(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLBATCHSTUDMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchStudMappingModel>(spName, new
            {
                Id = id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<int> InsertSectionStudMappings(List<BatchStudMappingModel> data)
        {
            var spName = ConstantSPnames.SP_INSERTBATCHSTUDMAP;
            var sendToDB = new ArrayList();
            foreach (var item in data)
            {
                sendToDB.Add(
                    new
                    {
                        BatchId = item.SectionId,
                        StudentId = item.StudentId,
                        CreatedBy = item.ModifiedBy,
                        CreatedDate = item.ModifiedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
            //return Task.Factory.StartNew(() => _db.Connection.Query<BatchStudMappingModel>(spName, new
            //{
            //    BatchId = model.BatchId,
            //    StudentId = model.StudentId,
            //    CreatedBy = model.CreatedBy,
            //    CreatedDate = model.CreatedDate

            //}, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<int> UpdateBatchStudMapping(List<BatchStudMappingModel> model)
        {
            var spName = ConstantSPnames.SP_UPDATEBATCHSTUDMAP;
            var sendToDB = new ArrayList();

            string sProc = ConstantSPnames.SP_UPDATEBATCHSTUDACTIVEMAP;
            var rowsUpdated = _db.Connection.Execute(sProc,
                new { BatchId = model.FirstOrDefault(x => x.SectionId != 0).SectionId },
                commandType: CommandType.StoredProcedure);
            foreach (var item in model)
            {
                sendToDB.Add(
                    new
                    {
                        Id = item.Id,
                        BatchId = item.SectionId,
                        StudentId = item.StudentId,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
            //_db.Connection.Query<BatchStudMappingModel>(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<int> DeleteBatchStudMapping(int[] ids, int batchId)
        {
            var delRecIds = new ArrayList();
            foreach (int id in ids)
            {
                delRecIds.Add(
                    new
                    {
                        Id = id
                    });
            }

            var spName1 = ConstantSPnames.SP_DELETEBATCHSTUDMAP;
            var spName = ConstantSPnames.SP_UPDATEBATCHSTUDACTIVEMAP;
            //  _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure))
            var rowsUpdated = _db.Connection.Execute(spName, new { BatchId = batchId },
                commandType: CommandType.StoredProcedure);
            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName1, delRecIds, commandType: CommandType.StoredProcedure));
            //return Task.Factory.StartNew(() => _db.Connection.Query<int>(spName, new[] { Id = id }, commandType: CommandType.StoredProcedure));

        }



        public async Task<string> bulkuploadstudent(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKSTUDENTUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync(); // Await this!

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentTable", SqlDbType.Structured).Value = target;

                SqlParameter returnStatusParam = command.Parameters.Add("@UploadStatus", SqlDbType.NVarChar, 50);
                returnStatusParam.Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync(); // Await this!

                return returnStatusParam.Value?.ToString() ?? string.Empty;
            }
            catch (SqlException ex)
            {
                return "SQL Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


        public string bulkuploadfaculty(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKSTUDENTUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                sqlConnection.OpenAsync();
                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@UserTable", SqlDbType.Structured).Value = target;

                SqlParameter returnStatusParam = command.Parameters.Add("@UploadStatus", SqlDbType.NVarChar, 50);
                returnStatusParam.Direction = ParameterDirection.Output;

                command.ExecuteNonQueryAsync();

                return (returnStatusParam.Value?.ToString() ?? string.Empty);
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string bulkuploadsubject(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKSTUDENTUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                sqlConnection.OpenAsync();
                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@UserTable", SqlDbType.Structured).Value = target;

                SqlParameter returnStatusParam = command.Parameters.Add("@UploadStatus", SqlDbType.NVarChar, 50);
                returnStatusParam.Direction = ParameterDirection.Output;

                command.ExecuteNonQueryAsync();

                return (returnStatusParam.Value?.ToString() ?? string.Empty);
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<List<AttendanceModel>> GetAllAttendance(DateTime? AttendanceDate, int sectionId, string Hoursday)
        {
            var spName = ConstantSPnames.SP_GETALLATTENDANCE;
            return Task.Factory.StartNew(() => _db.Connection.Query<AttendanceModel>(spName, new
            {
                AttendanceDate = AttendanceDate,
                SectionId = sectionId, 
                Hoursday = Hoursday

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public string InsertAttendance(List<AttendanceModel> attendance)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create("text.xml", settings))
            {
                writer.WriteStartElement("Paramters");
                if (attendance != null)
                {
                    for (int i = 0; i < attendance.Count; i++)
                    {
                        var hours = attendance[i].Hoursdays.Split(',');
                        for (int j = 0; j < hours.Length; j++)
                        {
                            writer.WriteStartElement("Param");
                            writer.WriteElementString("StudentId", attendance[i].StudentId.ToString());
                            writer.WriteElementString("SectionId", attendance[i].SectionId.ToString());
                            writer.WriteElementString("SubjectId", attendance[i].SubjectId.ToString());
                            writer.WriteElementString("Date", attendance[i].Date.ToString("MM/dd/yyyy"));
                            writer.WriteElementString("IsPresent", attendance[i].IsPresent.ToString());
                            writer.WriteElementString("Hoursday", hours[j].ToString());
                            writer.WriteElementString("CreatedBy", attendance[i].CreatedBy.ToString());
                            writer.WriteElementString("CreatedDate", attendance[i].CreatedDate.ToString());
                            writer.WriteEndElement();
                        }

                    }

                    writer.WriteEndElement();
                    writer.Close();
                    writer.Flush();
                    writer.Dispose();
                }

            }

            XmlReader xmlReader = new XmlTextReader("text.xml");
            string xml = File.ReadAllText("text.xml");
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);
            xmlReader.Close();
            var spName = ConstantSPnames.SP_INSERTATTENDANCE;
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(xml);

            using (SqlConnection sqlconnection =
                   new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                sqlconnection.Open();


                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("xml", SqlDbType.VarChar).Value = xml;
                command.CommandTimeout = 100000;
                command.ExecuteNonQuery();
                return "Success";
            }
        }

        public Task<List<AttendanceModel>> UpdateAttendance(AttendanceModel attendance)
        {
            var spName = ConstantSPnames.SP_UPDATEATTENDANCE;
            return Task.Factory.StartNew(() => _db.Connection.Query<AttendanceModel>(spName, new
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                SectionId = attendance.SectionId,
                SubjectId = attendance.SubjectId,
                Date = attendance.Date,
                IsPresent = attendance.IsPresent,
                ModifiedBy = attendance.ModifiedBy,
                ModifiedDate = attendance.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

        //public Task<List<BatchStudentSubjectList>> GetBatchWiseStudentList()
        //{
        //    var spName = ConstantSPnames.SP_GETALLSECTION;
        //    var batchStudSp = ConstantSPnames.SP_GETALLBATCHSTUDMAP;

        //    var batchDetails = _db.Connection.Query<SectionModel>(spName, commandType: CommandType.StoredProcedure)
        //        .ToList();
        //    var batchStudDetails = _db.Connection
        //        .Query<BatchStudMappingModel>(batchStudSp, commandType: CommandType.StoredProcedure).ToList();
        //    var batchStudMapping = batchStudDetails.GroupBy(x => x.BatchId);

        //    var result = from bs in batchStudDetails
        //                 join bd in batchDetails
        //                     on bs.BatchId equals bd.BatchId
        //                 select new BatchStudentMapping
        //                 {
        //                     BatchId = bs.BatchId,
        //                     BatchName = bd.BatchName,
        //                     ClassSection = bd.DeptSection,
        //                     Department = bd.Department,
        //                     Year = bd.Year,
        //                     Sem = bd.Sem,
        //                     StudentId = bs.StudentId,
        //                     StudentName = bs.StudentName,
        //                     SubjectId = bd.SubjectId,
        //                     Subject = bd.Subject

        //                 };
        //    List<BatchStudentSubjectList> batchStudentSubjectLists = new List<BatchStudentSubjectList>();
        //    var res1 = result.ToList().GroupBy(x => new
        //    { x.BatchId, x.BatchName, x.Department, x.ClassSection, x.Year, x.Sem });
        //    foreach (var item in res1)
        //    {
        //        List<StudentMap> students = new List<StudentMap>();
        //        List<SubjectMap> subjects = new List<SubjectMap>();
        //        foreach (var child in item)
        //        {
        //            students.Add(new StudentMap
        //            {
        //                StudentId = child.StudentId,
        //                StudentName = child.StudentName
        //            });
        //            subjects.Add(new SubjectMap
        //            {
        //                SubjectId = child.SubjectId,
        //                Subject = child.Subject
        //            });
        //        }

        //        batchStudentSubjectLists.Add(new BatchStudentSubjectList
        //        {
        //            BatchId = item.Key.BatchId,
        //            BatchName = item.Key.BatchName,
        //            ClassSection = item.Key.ClassSection,
        //            Department = item.Key.Department,
        //            Year = item.Key.Year,
        //            Sem = item.Key.Sem,
        //            Students = students,
        //            Subjects = subjects
        //        });
        //    }

        //    return Task.Factory.StartNew(() => batchStudentSubjectLists);
        //}

        public string DeleteAttendance(List<AttendanceModel> attendance)
        {
            var spName = ConstantSPnames.SP_DELETEATTENDANCE;
            var sendToDB = new ArrayList();
            try
            {
                foreach (var item in attendance)
                {
                    sendToDB.Add(
                        new
                        {
                            SectionId = item.SectionId,
                            StudentId = item.StudentId,
                            Hoursday = item.Hoursday,
                            SubjectId = item.SubjectId,
                            Date = item.Date.ToString("yyyy-MM-dd")
                        });
                }

                var delte = _db.Connection.Execute(spName, sendToDB.ToArray(),
                    commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public Task<int> PasswordReset(string userName, string password)
        {
            var spName = ConstantSPnames.SP_PASSWORDRESET;
            return Task.Factory.StartNew(() => _db.Connection.Execute(spName, new
            {
                UserName = userName,
                Password = password,
                ModifiedBy = userName,
                ModifiedDate = DateTime.Now

            }, commandType: CommandType.StoredProcedure));

        }

        //public Task<List<string>> GetSection(SectionModel sectionModel)
        //{
        //    var spName = ConstantSPnames.SP_GETSECTION;
        //    return Task.Factory.StartNew(() => _db.Connection.Query<string>(spName,
        //        new { @DepartmentId = sectionModel.DepartmentId, @Sem = sectionModel.Sem, @Year = sectionModel.Year },
        //        commandType: CommandType.StoredProcedure).ToList());
        //}

        public Task<List<Batchdetails>> GetBatchList(BatchListModel batchListModel)
        {
            var spName = ConstantSPnames.SP_GETBATCHLIST;
            return Task.Factory.StartNew(() => _db.Connection.Query<Batchdetails>(spName,
                new
                {
                    @DepartmentId = batchListModel.DepartmentId,
                    @Sem = batchListModel.Sem,
                    @Year = batchListModel.Year,
                    @DeptSection = batchListModel.DeptSection
                }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<BatchSubjectModel>> GetAllBatchSubMapping(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLBATCHSUBMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchSubjectModel>(spName, new
            {
                Id = id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<int> InsertBatchSubMappings(List<BatchSubjectModel> data)
        {
            var spName = ConstantSPnames.SP_INSERTBATCHSUBMAP;
            var sendToDB = new ArrayList();
            foreach (var item in data)
            {
                sendToDB.Add(
                    new
                    {
                        Name = item.Name,
                        BatchId = item.BatchId,
                        SubjectId = item.SubjectId,
                        DepartmentId = item.DepartmentId,
                        CreatedBy = item.ModifiedBy,
                        CreatedDate = item.ModifiedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
        }

        public Task<int> UpdateBatchSubMapping(List<BatchSubjectModel> model)
        {
            var spName = ConstantSPnames.SP_UPDATEBATCHSUBMAP;
            //var spDltUnmapSubAtt = ConstantSPnames.SP_DELUNMAPSUBATT;
            var sendToDB = new ArrayList();

            string sProc = ConstantSPnames.SP_UPDATEBATCHSUBACTIVEMAP;
            var rowsUpdated = _db.Connection.Execute(sProc,
                new { BatchId = model.FirstOrDefault(x => x.BatchId != 0).BatchId },
                commandType: CommandType.StoredProcedure);
            foreach (var item in model)
            {
                sendToDB.Add(
                    new
                    {
                        Id = item.Id,
                        Name = item.Name,
                        BatchId = item.BatchId,
                        SubjectId = item.SubjectId,
                        DepartmentId = item.DepartmentId,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
        }

        public Task<int> DeleteBatchSubMapping(int[] ids)
        {
            var delRecIds = new ArrayList();
            foreach (int id in ids)
            {
                delRecIds.Add(
                    new
                    {
                        Id = id
                    });
            }

            var spName = ConstantSPnames.SP_DELETEBATCHSUBMAP;

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, delRecIds, commandType: CommandType.StoredProcedure));

        }
        //public string generateAttendancereport()
        //{
        //    try
        //    {
        //        //string strfilepath = (ConfigurationManager.AppSettings["FilePath"].ToString() + "\\" + ConfigurationManager.AppSettings["FileName"].ToString() + "_" + DateTime.Now.ToShortDateString() + ".xlsx");
        //        //string strfilepath = (_appSettingse+ "\\" + _configuration.GetSection("AppSettings").GetSection("FileName").Value + ".xlsx");
        //        string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileName.ToString();
        //        DataTable dtTable = new DataTable();
        //        var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
        //        using (SqlConnection myConnection = new SqlConnection(con))
        //        {
        //            SqlCommand objCmd = new SqlCommand("sp_Attendance_MonthWise", myConnection);
        //            objCmd.CommandType = CommandType.StoredProcedure;
        //            //objCmd.Parameters.Add("@noofDays", SqlDbType.BigInt).Value = Noofdays;
        //            using (var da = new SqlDataAdapter(objCmd))
        //            {

        //                da.Fill(dtTable);
        //            }
        //        }

        //        using (XLWorkbook wb = new XLWorkbook())
        //        {

        //            wb.Worksheets.Add(dtTable, "Attendance_MonthWise").Columns().AdjustToContents();

        //            if (File.Exists(strfilepath))
        //            {
        //                File.Delete(strfilepath);
        //            }
        //            //lblerror.Text = "three";
        //            wb.SaveAs(strfilepath);// (filepath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            GC.Collect();
        //            //string strFileName = ConfigurationManager.AppSettings["FileName"];
        //            //Response.AppendHeader("Content-Disposition", "attachment; filename=" + strFileName);
        //            //Response.ContentType = "application/pdf";
        //            //Response.TransmitFile(ConfigurationManager.AppSettings["FilePath"].ToString() + strFileName);
        //            //Response.End();
        //        }
        //        return strfilepath;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }


        //}
      

        public Task<List<SubjectAttendanceModel>> Getsubjectsforattendance(string batch, string Department, string Sem,
            string Year, string Section)
        {
            var spName = ConstantSPnames.SP_Getsubjectsforattendance;
            return Task.Factory.StartNew(() => _db.Connection.Query<SubjectAttendanceModel>(spName, new
            {
                Department = Department,
                Sem = Sem,
                Year = Year,
                batchName = batch,
                Section = Section
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public string generateMonthlyAttendancereport(string Sem, string Year, int Department, DateTime AttendanceDate, string Section)
        {
            var spMonthwiseAtt = ConstantSPnames.SP_MonthwiseAttendance;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                 _appSettings.Settings.FileName.ToString();
            // DataTable dtCloned = new DataTable();
            string MonthTotal = "";
            string Cumulative = "";
            int i = 3;
            string Departmentname = string.Empty;
            string AttDate = String.Format("{0:MMMM}", AttendanceDate).ToUpper() + ", " + String.Format("{0:yyyy}", AttendanceDate);
            var YearLtr = Year == "1" ? "I" : Year == "2" ? "II" : "III";
            string Semester = YearLtr + "/" + Sem.ToUpper();
            List<String> SubjectCode = new List<String>();
            List<String> SubjectHours = new List<String>();
            try
            {

                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spMonthwiseAtt, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@AttMon", SqlDbType.VarChar).Value = String.Format("{0:MM}", AttendanceDate);
                        objCmd.Parameters.Add("@AttYear", SqlDbType.VarChar).Value = String.Format("{0:yyyy}", AttendanceDate);
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = Department;
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        objCmd.Parameters.Add("@Date", SqlDbType.VarChar).Value = (AttendanceDate).ToString("yyyy-MM-dd");
                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        var dataTable = ds.Tables[0];
                        // Create a new DataTable for the transformed data
                        var transformedTable = new DataTable();

                        // Define the columns to be added
                        var columnsToAdd = new List<string> { "SNo", "StudentID", "StudentName" };
                        List<string> batchNameList = dataTable.AsEnumerable()
                                        .Select(row => row.Field<string>("BatchName"))
                                        .Distinct()
                                        .ToList();
                        //dataTable.Columns.Remove("BatchName");
                        // Identify dynamic columns
                        var dynamicColumns = new List<string>();
                        // Identify dynamic columns
                        var batchTotalCount = new List<string>();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (column.ColumnName == "DepartmentName")
                            {
                                Departmentname = dataTable.Rows[0][column].ToString();
                                continue;
                            }
                            if (column.ColumnName == "BatchName")
                            {
                                continue;
                            }
                            if (column.ColumnName == "SNo" || column.ColumnName == "StudentID" || column.ColumnName == "StudentName")
                            {
                                columnsToAdd.Add(column.ColumnName);
                                transformedTable.Columns.Add(column.ColumnName);
                            }
                            else if (column.ColumnName.Contains('|') || column.ColumnName == "Attended_Hours" || column.ColumnName == "SemAttended_Hours")
                            {
                                var parts = column.ColumnName.Split('|');
                                dynamicColumns.Add(column.ColumnName);
                                transformedTable.Columns.Add(parts[0] + "_Part1");
                                transformedTable.Columns.Add(parts[0] + "_Part2");
                            }
                        }



                        // Copy and transform data
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var newRow = transformedTable.NewRow();
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                if (column.ColumnName == "DepartmentName" || column.ColumnName == "BatchName")
                                {
                                    continue; // Skip DepartmentName column
                                }

                                var value = row[column].ToString();

                                if (columnsToAdd.Contains(column.ColumnName))
                                {
                                    newRow[column.ColumnName] = value;
                                }
                                else if (dynamicColumns.Contains(column.ColumnName))
                                {

                                    var parts = value.Split('|');
                                    if (parts.Length == 2)
                                    {
                                        var part1 = Convert.ToDouble(parts[0]);
                                        var part2 = Convert.ToDouble(parts[1]);
                                        //batchTotalCount.Add(part2.ToString());
                                        var colSplit = column.ColumnName.Split('|');
                                        // Add the part1 value to the new row
                                        newRow[colSplit[0] + "_Part1"] = part1;

                                        // Calculate the percentage and store it in _Part2
                                        double percentage = part2 != 0 ? (part1 / part2) * 100 : 0;
                                        newRow[colSplit[0] + "_Part2"] = percentage.ToString("F0"); // Format to 2 decimal places
                                    }

                                    else
                                    {
                                        var colSplit = column.ColumnName.Split('|');
                                        // Add the part1 value to the new row
                                        newRow[colSplit[0] + "_Part1"] = 0;
                                        newRow[colSplit[0] + "_Part2"] = 0;
                                    }
                                }
                            }
                            transformedTable.Rows.Add(newRow);
                        }
                        // Dictionary to store distinct values for each column
                        var subjectwiseTotalAtt = new Dictionary<string, List<string>>();

                        // Iterate through each column in the DataTable



                        // Iterate through each column in the DataTable
                        foreach (var column in dynamicColumns)
                        {
                            // Create a new list for each column iteration to hold distinct values
                            var columnDistinctValues = new List<string>();

                            // Get distinct values for the current column
                            for (int j = 0; j < batchNameList.Count; j++)
                            {
                                var batchValue = dataTable
                                    .AsEnumerable() // Convert DataTable to Enumerable
                                    .Where(row => row["BatchName"].ToString() == batchNameList[j])
                                    .Select(row => row[column].ToString().Split('|').LastOrDefault())
                                    .Distinct()
                                    .FirstOrDefault();  // Get the first distinct value

                                columnDistinctValues.Add(batchNameList[j] + '-' + batchValue);
                            }

                            // Add the distinct values to the dictionary, ensuring the column name is used as the key
                            subjectwiseTotalAtt.Add(column.Split('|').FirstOrDefault(), columnDistinctValues);
                        }


                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            i = transformedTable.Columns.Count + 1; //Column Max
                            int j = i / 3;
                            int j1 = j * 2;
                            int j2 = j * 3;
                            int rowNumber = 1;
                            int ColNumber = 1;
                            var ws = wb.Worksheets.Add("Attendance_MonthWise_Dynamic");
                            int count = subjectwiseTotalAtt.Count();
                            ws.Cell(rowNumber, ColNumber).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                            ws.Range(rowNumber, 1, rowNumber, i).Merge().AddToNamed("Titles");
                            rowNumber++;
                            ws.Cell(rowNumber, 1).Value = "STUDENT ATTENDANCE PARTICULARS";
                            ws.Range(rowNumber, 1, rowNumber, i).Merge().AddToNamed("Titles");
                            rowNumber++;
                            ws.Cell(rowNumber, ColNumber).Value = "PROGRAMME: " + Departmentname.ToUpper();
                            ws.Range(rowNumber, 1, rowNumber, j - 1).Merge().AddToNamed("Titles");
                            ws.Cell(rowNumber, j).Value = "MONTH: " + AttDate;
                            ws.Range(rowNumber, j, rowNumber, j1 - 1).Merge().AddToNamed("Titles");
                            ws.Cell(rowNumber, j1).Value = "SEMESTER: " + Semester;
                            ws.Range(rowNumber, j1, rowNumber, i).Merge().AddToNamed("Titles");
                            rowNumber++;
                            ws.Cell(rowNumber, ColNumber).Value = "S.No";
                            //ws.Columns(4, 1).AdjustToContents();
                            ColNumber++;
                            ws.Range("A4:A6").Merge().AddToNamed("Titles");
                            ws.Cell(rowNumber, ColNumber).Value = "Student Id";
                            ws.Range("B4:B6").Merge().AddToNamed("Titles");
                            ws.Columns(rowNumber, ColNumber).Width = 3;
                            //ws.Columns(4, 2).AdjustToContents();
                            ColNumber++;
                            ws.Cell(rowNumber, ColNumber).Value = "Course Code";
                            //ws.Cell("C4").Style.Alignment.WrapText = true;
                            rowNumber++;
                            ws.Range("C4").Merge().AddToNamed("Titles");
                            ws.Cell(rowNumber, ColNumber).Value = " Hours handled for this month";
                            ws.Range("C5").Merge().AddToNamed("Titles");
                            rowNumber++;
                            ws.Cell(rowNumber, ColNumber).Value = "Name of the Canditate";
                            ws.Range("C6").Merge().AddToNamed("Titles");
                            rowNumber++;
                            var rangeWithData = ws.Cell(rowNumber, 1).InsertData(transformedTable.AsEnumerable());
                            rangeWithData.Style.Font.SetFontSize(7);
                            ws.Cell(4, i).Value = "Student Signature";
                            ws.Columns(4, i).Width = 3;
                            ws.Range(4, i, 6, i).Column(1).Merge().AddToNamed("Titles");
                            ws.Cell(4, (i - 4)).Value = "Curr.Month Total: " + string.Join(",", subjectwiseTotalAtt.ElementAt(count - 2).Value);
                            ws.Columns(4, i - 4).Width = 3;
                            ws.Range(4, (i - 4), 4, i - 3).Merge().AddToNamed("Titles");
                            ws.Cell(5, (i - 4)).Value = "Attended Hours";
                            ws.Range(5, (i - 4), 6, (i - 4)).Column(1).Merge().AddToNamed("Titles");
                            ws.Cell(5, (i - 3)).Value = "%";
                            ws.Range(5, (i - 3), 6, (i - 3)).Column(1).Merge().AddToNamed("Titles");
                            //ws.Cell(5, i - 4).Value = "%";
                            //ws.Range(5, i - 4, 6, i - 4).Column(1).Merge().AddToNamed("Titles");
                            ws.Cell(4, (i - 2)).Value = "Cumulative: " + string.Join(",", subjectwiseTotalAtt.ElementAt(count - 1).Value);
                            ws.Range(4, i - 2, 4, i - 1).Merge().AddToNamed("Titles");
                            ws.Columns(4, i - 2).Width = 3;
                            ws.Cell(5, (i - 2)).Value = "Attended Hours";
                            ws.Range(5, (i - 2), 6, (i - 2)).Merge().AddToNamed("Titles");
                            ws.Cell(5, (i - 1)).Value = "%";
                            ws.Range(5, (i - 1), 6, (i - 1)).Merge().AddToNamed("Titles");
                            int RowCount = ds.Tables[0].Rows.Count + rowNumber;
                            ws.Cell(RowCount, 1).Value = "SIGNATURE";
                            ws.Range(RowCount, 1, RowCount, 3).Merge().AddToNamed("Titles");
                            ws.Cell(RowCount + 1, 1).Value = "NAME OF THE STAFF";
                            ws.Range(RowCount + 1, 1, RowCount + 1, 3).Merge().AddToNamed("Titles");
                            subjectwiseTotalAtt.Remove("Attended_Hours");
                            subjectwiseTotalAtt.Remove("SemAttended_Hours");
                            int y = 4;
                            foreach (var sub in subjectwiseTotalAtt)
                            {
                                ws.Cell(4, y).Value = sub.Key;
                                ws.Range(4, y, 4, y + 1).Merge().AddToNamed("Titles");
                                ws.Columns(4, y + 1).Width = 3;
                                ws.Range(5, y, 5, y + 1).Merge().AddToNamed("Titles");
                                ws.Columns(5, y + 1).Width = 3;
                                ws.Cell(5, y).Value = "";
                                ws.Cell(5, y).Value = string.Join(",", sub.Value);
                                //ws.Cell(RowCount, y).Value = " ";
                                ws.Range(RowCount, y, RowCount, y + 1).Merge().AddToNamed("Titles");
                                ws.Cell(RowCount + 1, y).Value = " ";
                                ws.Range(RowCount + 1, y, RowCount + 1, y + 1).Merge().AddToNamed("Titles");
                                ws.Cell(6, y).Value = "HP";
                                ws.Range(6, y, 6, y).Merge().AddToNamed("Titles");
                                ws.Cell(6, y + 1).Value = "%";
                                ws.Range(6, y + 1, 6, y + 1).Merge().AddToNamed("Titles");
                                y = y + 2;
                            }

                            var titlesStyle = wb.Style;
                            titlesStyle.Font.Bold = true;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Alignment.WrapText = true;
                            titlesStyle.Font.SetFontSize(7);


                            //wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                            var namedRange = wb.NamedRanges.NamedRange("Titles");

                            if (namedRange != null)
                            {
                                namedRange.Ranges.Style = titlesStyle;
                            }
                            else
                            {
                                Console.WriteLine("Named range 'Titles' not found.");
                            }
                            IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(RowCount + 1, i).Address);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            ws.Column(1).AdjustToContents();

                            if (File.Exists(strfilepath))
                            {
                                File.Delete(strfilepath);
                            }

                            //lblerror.Text = "three";
                            wb.SaveAs(strfilepath);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();

                        }

                        return strfilepath;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex.InnerException.ToString());

                return ex.Message;
            }
        }

        public string generateAttendancedynamicreport(string Sem, string Year, int Department, string Section)
        {
            try
            {
                var spName = ConstantSPnames.SP_MonthwiseDynamicAttendance;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    //objCmd.Parameters.Add("@noofDays", SqlDbType.BigInt).Value = Noofdays;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = Convert.ToString(Department);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        da.Fill(dtTable);
                    }
                }


                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtTable, "Attendance_Dynamic_Report").Columns().AdjustToContents();

                    if (File.Exists(strfilepath))
                    {
                        File.Delete(strfilepath);
                    }
                    //lblerror.Text = "three";
                    wb.SaveAs(strfilepath); // (filepath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                return strfilepath;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string generateAttendancesubjectwisereport(string SubjectCode, string Sem, string Year,
            string DepartmentId)
        {
            try
            {
                var spName = ConstantSPnames.SP_SubjectwiseAttendance;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;

                    //objCmd.Parameters.Add("@noofDays", SqlDbType.BigInt).Value = Noofdays;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        objCmd.Parameters.Add("@SubjectCode", SqlDbType.VarChar).Value = SubjectCode;
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@DepartmentId", SqlDbType.VarChar).Value = DepartmentId;
                        da.Fill(dtTable);

                    }
                }

                //if (dtTable.Columns.Count != 0)
                //{
                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dtTable, "Attendance_SubjectWise_Monthly").Columns().AdjustToContents();

                    if (File.Exists(strfilepath))
                    {
                        File.Delete(strfilepath);
                    }

                    //lblerror.Text = "three";
                    wb.SaveAs(strfilepath); // (filepath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    //string strFileName = ConfigurationManager.AppSettings["FileName"];
                    //Response.AppendHeader("Content-Disposition", "attachment; filename=" + strFileName);
                    //Response.ContentType = "application/pdf";
                    //Response.TransmitFile(ConfigurationManager.AppSettings["FilePath"].ToString() + strFileName);
                    //Response.End();
                }

                return strfilepath;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public string generateAttendanceSemwisereport(string Sem, string Year, int Department, string Section, string AcademicFrom, string AcademicTo)
        {
            try
            {
                string Departmentname = "";
               // var spGetDepartmentName = ConstantSPnames.SP_GetDepartmentName;
                var spSemwisewiseAtt = ConstantSPnames.SP_SemWiseAttendanceReports;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                string Cumulative = "";
                string Cumulative1 = "";
                int i = 0;
                int SNo = 1;
                var YearLtr = Year == "1" ? "I" : Year == "2" ? "II" : "III";
                string Semester = YearLtr + "/" + Sem.ToUpper();
                List<String> SubjectCode = new List<String>();
                string[] SubjectHours;
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();

                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spSemwisewiseAtt, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = Convert.ToString(Department);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        objCmd.Parameters.Add("@AcadamicFrom", SqlDbType.VarChar).Value = AcademicFrom;
                        objCmd.Parameters.Add("@AcadamicTo", SqlDbType.VarChar).Value = AcademicTo;
                        da.Fill(ds);
                        List<string> SubId = new List<string>();
                        IList<MonthyAttModel> SubjectHourAtt = ds.Tables[1].AsEnumerable().Select(row =>
                            new MonthyAttModel
                            {
                                BatchId = row.Field<int>("BatchId"),
                                SemTotalHours = row.Field<int>("SemTotalHours"),
                                SubjectId = row.Field<long>("SubjectId"),
                                BatchName = row.Field<string>("BatchName")
                            }).ToList();

                        Departmentname = ds.Tables[2].Rows[0].ItemArray[0].ToString();
                        foreach (DataColumn dc in ds.Tables[0].Columns)
                        {
                            var Column = dc.ToString();
                            if (Column != "BatchId" && Column != "StudentID" && Column != "StudentName" &&
                                Column != "Attended_Hours" && Column != "Total_Hours")
                            {
                                dtTable.Columns.Add(Column);
                                char[] ch = { '|' };
                                string[] words = Column.Split(ch);
                                SubjectCode.Add(words[0]);
                                dtTable.Columns.Add(words[0] + "-%");
                                SubId.Add(words[1]);
                            }
                            else
                            {
                                dtTable.Columns.Add(Column);
                            }
                        }

                        SubjectHours = new string[SubId.Count];
                        for (int k = 0; k < SubjectHourAtt.Count; k++)
                        {
                            string BatchName = Convert.ToString(SubjectHourAtt[k].BatchName);
                            string SubjectId = Convert.ToString(SubjectHourAtt[k].SubjectId);
                            string SemTotalHours = Convert.ToString(SubjectHourAtt[k].SemTotalHours);
                            for (int s = 0; s < SubId.Count; s++)
                            {
                                if (SubjectId == SubId[s])
                                {
                                    if (SubjectHours[s] == null)
                                    {
                                        SubjectHours[s] = BatchName + ":" + SemTotalHours;
                                    }
                                    else
                                    {
                                        SubjectHours[s] += ", " + BatchName + ":" + SemTotalHours;
                                    }

                                }
                            }
                        }

                        foreach (DataRow dtRow in ds.Tables[0].Rows)
                        {
                            int SubCnt = 0;
                            int drIndex = 1;
                            object[] list = new object[dtTable.Columns.Count];
                            list[0] = SNo;
                            SNo++;

                            for (int j = 1; j < ((dtRow.ItemArray.Length - 5) + dtRow.ItemArray.Length); j++)
                            {
                                if (j > 2)
                                {
                                    decimal Percentage = 0;
                                    list[j] = dtRow.ItemArray[drIndex];
                                    if (SubCnt != SubId.Count)
                                    {
                                        int TotalHr = 0;
                                        TotalHr = SubjectHourAtt
                                            .Where(A => A.SubjectId == Convert.ToInt32(SubId[SubCnt]) &&
                                                        A.BatchId == Convert.ToInt32(dtRow.ItemArray[0]))
                                            .Select(A => A.SemTotalHours).DefaultIfEmpty(1).FirstOrDefault();
                                        Percentage =
                                            Math.Round((Convert.ToDecimal(dtRow.ItemArray[drIndex]) / TotalHr) * 100,
                                                1);
                                        SubCnt++;
                                    }
                                    else
                                    {
                                        Percentage =
                                            Math.Round(
                                                (Convert.ToDecimal(dtRow.ItemArray[drIndex]) /
                                                 Convert.ToInt32(dtRow.ItemArray[drIndex + 1])) * 100, 1);
                                        if (Cumulative == "")
                                        {
                                            Cumulative = Convert.ToString(dtRow.ItemArray[drIndex + 1]);
                                            Cumulative1 = Convert.ToString(dtRow.ItemArray[drIndex + 1]);
                                        }
                                        else if (Cumulative1 != Convert.ToString(dtRow.ItemArray[drIndex + 1]))
                                        {
                                            Cumulative += "," + Convert.ToString(dtRow.ItemArray[drIndex + 1]);
                                            Cumulative1 = Convert.ToString(dtRow.ItemArray[drIndex + 1]);
                                        }

                                        drIndex++;
                                    }

                                    j++;
                                    list[j] = Percentage;
                                    drIndex++;
                                }
                                else
                                {
                                    list[j] = dtRow.ItemArray[drIndex];
                                    drIndex++;
                                }
                            }

                            dtTable.Rows.Add(list);
                        }
                    }
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    i = dtTable.Columns.Count + 1;
                    int j = i / 3;
                    int j1 = j * 2;
                    int j2 = j * 3;

                    var ws = wb.Worksheets.Add("Attendance_SemWise_Report");
                    ws.Cell(1, 1).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                    ws.Range(1, 1, 1, i).Merge().AddToNamed("Titles");
                    ws.Cell(2, 1).Value = "STUDENT ATTENDANCE PARTICULARS";
                    ws.Range(2, 1, 2, i).Merge().AddToNamed("Titles");
                    ws.Cell(3, 1).Value = "PROGRAMME: " + Departmentname.ToUpper();
                    ws.Range(3, 1, 3, j - 1).Merge().AddToNamed("Titles");
                    ws.Cell(3, j).Value = "Section: " + Section;
                    ws.Range(3, j, 3, j1 - 1).Merge().AddToNamed("Titles");
                    ws.Cell(3, j1).Value = "SEMESTER: " + Semester;
                    ws.Range(3, j1, 3, j2 - 1).Merge().AddToNamed("Titles");
                    ws.Cell(4, 1).Value = "S.No";
                    //ws.Columns(4, 1).AdjustToContents();
                    ws.Range("A4:A6").Merge().AddToNamed("Titles");
                    ws.Cell(4, 2).Value = "Student Id";
                    ws.Range("B4:B6").Merge().AddToNamed("Titles");
                    ws.Columns(4, 2).Width = 3;
                    //ws.Columns(4, 2).AdjustToContents();
                    ws.Cell(4, 3).Value = "Course Code";
                    //ws.Cell("C4").Style.Alignment.WrapText = true;
                    ws.Range("C4").Merge().AddToNamed("Titles");
                    ws.Cell(5, 3).Value = " Hours handled for this month";
                    ws.Range("C5").Merge().AddToNamed("Titles");
                    ws.Cell(6, 3).Value = "Name of the Canditate";
                    ws.Range("C6").Merge().AddToNamed("Titles");

                    var rangeWithData = ws.Cell(7, 1).InsertData(dtTable.AsEnumerable());
                    rangeWithData.Style.Font.SetFontSize(5);
                    ws.Cell(4, i).Value = "Student Signature";
                    ws.Columns(4, i).Width = 3;
                    ws.Range(4, i, 6, i).Column(1).Merge().AddToNamed("Titles");
                    //ws.Cell(5, i - 4).Value = "%";
                    //ws.Range(5, i - 4, 6, i - 4).Column(1).Merge().AddToNamed("Titles");
                    ws.Cell(4, (i - 2)).Value = "Cumulative: " + Cumulative;
                    ws.Range(4, i - 2, 4, i - 1).Merge().AddToNamed("Titles");
                    ws.Columns(4, i - 2).Width = 3;
                    ws.Cell(5, (i - 2)).Value = "Attended Hours";
                    ws.Range(5, (i - 2), 6, (i - 2)).Merge().AddToNamed("Titles");
                    ws.Cell(5, (i - 1)).Value = "%";
                    ws.Range(5, (i - 1), 6, (i - 1)).Merge().AddToNamed("Titles");


                    int RowCount = dtTable.Rows.Count + 7;
                    ws.Cell(RowCount, 1).Value = "SIGNATURE";
                    ws.Range(RowCount, 1, RowCount, 3).Merge().AddToNamed("Titles");
                    ws.Cell(RowCount + 1, 1).Value = "NAME OF THE STAFF";
                    ws.Range(RowCount + 1, 1, RowCount + 1, 3).Merge().AddToNamed("Titles");

                    int y = 4;
                    for (int x = 0; x < SubjectCode.Count; x++)
                    {
                        ws.Cell(4, y).Value = SubjectCode[x];
                        ws.Range(4, y, 4, y + 1).Merge().AddToNamed("Titles");
                        ws.Columns(4, y + 1).Width = 3;
                        ws.Cell(5, y).Value = SubjectHours[x];
                        ws.Range(5, y, 5, y + 1).Merge().AddToNamed("Titles");
                        ws.Cell(RowCount, y).Value = " ";
                        ws.Range(RowCount, y, RowCount, y + 1).Merge().AddToNamed("Titles");
                        ws.Cell(RowCount + 1, y).Value = " ";
                        ws.Range(RowCount + 1, y, RowCount + 1, y + 1).Merge().AddToNamed("Titles");
                        ws.Cell(6, y).Value = "HP";
                        ws.Range(6, y, 6, y).Merge().AddToNamed("Titles");
                        ws.Cell(6, y + 1).Value = "%";
                        ws.Range(6, y + 1, 6, y + 1).Merge().AddToNamed("Titles");
                        y = y + 2;
                    }

                    var titlesStyle = wb.Style;
                    titlesStyle.Font.Bold = true;
                    titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Alignment.WrapText = true;
                    titlesStyle.Font.SetFontSize(6);
                    //wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                    var namedRange = wb.NamedRanges.NamedRange("Titles");

                    if (namedRange != null)
                    {
                        namedRange.Ranges.Style = titlesStyle;
                    }
                    else
                    {
                        Console.WriteLine("Named range 'Titles' not found.");
                    }
                    IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(RowCount + 1, i).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Column(1).AdjustToContents();


                    if (File.Exists(strfilepath))
                    {
                        File.Delete(strfilepath);
                    }

                    //lblerror.Text = "three";
                    wb.SaveAs(strfilepath);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                }

                return strfilepath;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }
        }

        public string generateSubjectwiseMonthlyreport(string Sem, string Year, int Department, DateTime AttendanceDate,
            string Section, string SubjectCode)
        {
            try
            {
                var spName = ConstantSPnames.SP_SubjectwiseMonthlyDynamicAttendance;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
                DataTable dtTable = new DataTable();
                List<string[]> listOfData = new List<string[]>();
                //var listOfData = new List<string>();
                string DeptName = null;
                string BatchName = null;
                string MonthTotal = null;
                string CumuTotal = null;
                int i = 0;
                int SNo = 1;
                var YearLtr = Year == "1" ? "I" : Year == "2" ? "II" : "III";
                string Semester = YearLtr + "/" + Sem.ToUpper();
                string MonYr = String.Format("{0:MMMM}", AttendanceDate).ToUpper() + ", " +
                               String.Format("{0:yyyy}", AttendanceDate);
                int DaysInMon = DateTime.DaysInMonth(Convert.ToInt32(String.Format("{0:yyyy}", AttendanceDate)),
                    Convert.ToInt32(String.Format("{0:MM}", AttendanceDate)));
                string[] DataCol = new string[DaysInMon + 6];
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@Department", SqlDbType.VarChar).Value = Convert.ToString(Department);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        objCmd.Parameters.Add("@SubjectCode", SqlDbType.VarChar).Value = SubjectCode;
                        objCmd.Parameters.Add("@AttMon", SqlDbType.VarChar).Value =
                            String.Format("{0:MM}", AttendanceDate);
                        objCmd.Parameters.Add("@AttYear", SqlDbType.VarChar).Value =
                            String.Format("{0:yyyy}", AttendanceDate);
                        objCmd.Parameters.Add("@Date", SqlDbType.VarChar).Value =
                            (AttendanceDate).ToString("yyyy-MM-dd");
                        da.Fill(ds);

                        IList<DailyAttModel> PeriodPerDate = ds.Tables[1].AsEnumerable().Select(row =>
                            new DailyAttModel
                            {
                                Date = row.Field<DateTime>("Date"),
                                Hoursday = row.Field<int>("Hoursday")
                            }).ToList();
                        DeptName = Convert.ToString(ds.Tables[2].Rows[0].ItemArray[0]);
                        DataCol[i] = "S.No";
                        foreach (DataColumn dc in ds.Tables[0].Columns)
                        {
                            string Column = Convert.ToString(dc);
                            if (Column != "Total_CA" && Column != "Total_MA" && Column != "BatchName")
                            {
                                DateTime dDate;
                                if (DateTime.TryParse(Column, out dDate)) //Condition to check column name is date
                                {
                                    string[] words = Column.Split('-');
                                    i++;
                                    int[] Hour = PeriodPerDate.Where(A => A.Date == Convert.ToDateTime(Column))
                                        .Select(A => A.Hoursday).ToArray();
                                    int PeriodCnt = 1;
                                    string DateClm = words[2] + "-(" + Convert.ToString(Hour[0]);
                                    while (Hour.Length != PeriodCnt)
                                    {
                                        DateClm = DateClm + "," + Convert.ToString(Hour[PeriodCnt]);
                                        ++PeriodCnt;
                                    }

                                    DateClm = DateClm + ")";
                                    DataCol[i] = DateClm;
                                }
                                else
                                {
                                    i++;
                                    DataCol[i] = Column;
                                }

                            }

                        }

                        DataCol[i + 1] = "CPA";
                        listOfData.Add(DataCol);
                        string Batch = null;
                        foreach (DataRow dtRow in ds.Tables[0].Rows)
                        {
                            i = 0;
                            string[] Data = new string[DaysInMon + 6];

                            Data[i] = Convert.ToString(SNo);
                            for (int k = 1; k < dtRow.ItemArray.Length; k++)
                            {

                                if (k == dtRow.ItemArray.Length - 3) //Condition to check column value is MonthTotal
                                {
                                    if (MonthTotal == null)
                                    {
                                        MonthTotal = Convert.ToString(dtRow.ItemArray[k]);
                                        Batch = Convert.ToString(dtRow.ItemArray[0]);
                                    }
                                    else if (Batch != Convert.ToString(dtRow.ItemArray[0]))
                                    {
                                        MonthTotal = MonthTotal + ", " + Convert.ToString(dtRow.ItemArray[k]);
                                        Batch = Convert.ToString(dtRow.ItemArray[0]);
                                    }
                                }
                                else if
                                    (k == dtRow.ItemArray.Length -
                                     1) //Condition to check column value is CumulativeTotal
                                {
                                    if (CumuTotal == null)
                                    {
                                        CumuTotal = Convert.ToString(dtRow.ItemArray[k]);
                                        BatchName = Convert.ToString(dtRow.ItemArray[0]);
                                    }
                                    else if (BatchName != Convert.ToString(dtRow.ItemArray[0]))
                                    {
                                        CumuTotal = CumuTotal + ", " + Convert.ToString(dtRow.ItemArray[k]);
                                        BatchName = Convert.ToString(dtRow.ItemArray[0]);
                                    }

                                    i++;
                                    //Data[i] = Convert.ToString(dtRow.ItemArray[k]);

                                    decimal Percentage =
                                        Math.Round(
                                            (Convert.ToDecimal(dtRow.ItemArray[k - 1]) /
                                             Convert.ToDecimal(dtRow.ItemArray[k])) * 100, 1);
                                    //i++;
                                    Data[i] = Convert.ToString(Percentage);
                                }
                                else if ((k == dtRow.ItemArray.Length - 2) || (k == dtRow.ItemArray.Length - 4) ||
                                         (k == 1) ||
                                         (k == 2)) //Condition to check column value is studentId, StudentName, TotalAttended, CumulativeAttended
                                {
                                    i++;
                                    Data[i] = Convert.ToString(dtRow.ItemArray[k]);
                                }
                                else
                                {
                                    i++; //For Date values decoded the values to a and /                              
                                    Data[i] = Convert.ToString(dtRow.ItemArray[k]) == "0" ? "a" : "/";

                                }

                            }

                            SNo++;
                            listOfData.Add(Data);

                        }
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        int datalength = i;
                        i = DaysInMon + 6;
                        int j = i / 4;
                        int j1 = j * 2;
                        int j2 = j * 3;
                        int dataRow = listOfData.Count + 6;

                        var ws = wb.Worksheets.Add("Attendance_SubjectWise_Daily");
                        ws.AddPicture(imagePath).MoveTo(ws.Cell(1, 1));
                        ws.Cell(1, 1).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                        ws.Range(1, 1, 1, i).Merge().AddToNamed("Titles");
                        ws.Cell(2, 1).Value =
                            "| Govt Aided | Autonomous | NBA Attended: Civil, Mech, EEE, Prod. & Textile Tech Programmes|";
                        ws.Range(2, 1, 2, i).Merge().AddToNamed("Titles");
                        ws.Cell(3, 1).Value =
                            "Ranked as BEST PROGRAMMER in ARIIA Ranking 2021 | National Level CII Industrial Innovation Award 2021";
                        ws.Range(3, 1, 3, i).Merge().AddToNamed("Titles");
                        ws.Cell(4, 1).Value = "ATTENDANCE SHEET";
                        ws.Range(4, 1, 4, i).Merge().AddToNamed("Titles");

                        ws.Cell(5, 1).Value = "Semester/Year: " + Semester;
                        ws.Range(5, 1, 5, j - 1).Merge().AddToNamed("Titles");
                        ws.Cell(5, j).Value = "Programme: " + DeptName.ToUpper();
                        ws.Range(5, j, 5, j1 - 1).Merge().AddToNamed("Titles");
                        ws.Cell(5, j1).Value = "Course Code: " + SubjectCode;
                        ws.Range(5, j1, 5, j2 - 1).Merge().AddToNamed("Titles");
                        ws.Cell(5, j2).Value = "Attendance Month/Year: " + MonYr;
                        ws.Range(5, j2, 5, i).Merge().AddToNamed("Titles");

                        var rangeWithData = ws.Cell(6, 1).InsertData(listOfData);
                        rangeWithData.Style.Font.SetFontSize(8);
                        ws.Cell(dataRow, 1).Value = "Period Handled for this month: " + MonthTotal;
                        ws.Range(dataRow, 1, dataRow, (i / 2) - 1).Merge().AddToNamed("Titles");

                        ws.Cell(dataRow, (i / 2)).Value = "Cumulative total Hours/ Period: " + CumuTotal;
                        ws.Range(dataRow, (i / 2), dataRow, i).Merge().AddToNamed("Titles");

                        var titlesStyle = wb.Style;
                        titlesStyle.Font.Bold = true;
                        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Alignment.WrapText = true;
                        titlesStyle.Font.SetFontSize(10);


                        //wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                        var namedRange = wb.NamedRanges.NamedRange("Titles");

                        if (namedRange != null)
                        {
                            namedRange.Ranges.Style = titlesStyle;
                        }
                        else
                        {
                            Console.WriteLine("Named range 'Titles' not found.");
                        }
                        IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(dataRow, i).Address);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        ws.Columns().AdjustToContents();
                        ws.Columns(datalength + 1, i).Width = 3;
                        if (File.Exists(strfilepath))
                        {
                            File.Delete(strfilepath);
                        }

                        //lblerror.Text = "three";
                        wb.SaveAs(strfilepath);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();

                    }

                    return strfilepath;

                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }


        }

        public Task<List<FacultyModel>> GetVerifyPassword(string UserName, string Password)
        {
            var spName = ConstantSPnames.SP_GETVERIFYPASSWORD;
            return Task.Factory.StartNew(() => _db.Connection.Query<FacultyModel>(spName, new
            {
                UserName = UserName,
                Password = Password,

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public string UpdateVerifyPassword(string UserName, string NewPassword, string OldPassword, long FacultyId)
        {
            var spName = ConstantSPnames.SP_UPDATEVERIFYPASSWORD;
            try
            {
                using (SqlConnection sqlconnection =
                       new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {

                    sqlconnection.Open();

                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("UserName", SqlDbType.VarChar).Value = UserName;
                    command.Parameters.Add("Oldpassword", SqlDbType.VarChar).Value = OldPassword;
                    command.Parameters.Add("Newpassword", SqlDbType.VarChar).Value = NewPassword;
                    command.Parameters.Add("FacultyId", SqlDbType.BigInt).Value = FacultyId;

                    command.ExecuteNonQuery();
                    sqlconnection.Close();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "2627")
                {
                    return "User name mis";
                }
                return ex.Message.ToString();
            }
            //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());


            //return Task.Factory.StartNew(() => _db.Connection.Query<FacultyModel>(spName, new
            //{
            //    DepartmentId = DepartmentId,
            //    RoleId = RoleId,
            //    UserName = UserName,
            //    Oldpassword = OldPassword,
            //    Newpassword = NewPassword
            //}, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<IndentModel>> GetAllIndentDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETAllINDENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<IndentModel>(spName,
                new { RefNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<IndentModel>> InsertIndentDetails(IndentModel indentModel)
        {
            var spName = ConstantSPnames.SP_INSERTINDENT;
            //indentModel.MakerDate = indentModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;

            return Task.Factory.StartNew(() => _db.Connection.Query<IndentModel>(spName, new
            {
                FormDate = indentModel.FormDate,
                ReferenceNo = indentModel.ReferenceNo,

                NameOfTheProgramme = indentModel.NameOfTheProgramme,
                NameOfTheLaboratory = indentModel.NameOfTheLaboratory,
                HeadOfAccount = indentModel.HeadOfAccount,
                purpose = indentModel.purpose,
                nature = indentModel.nature,
                collegeType = indentModel.CollegeType,
                FacultyName = indentModel.FacultyName,
                requirement = indentModel.requirement,
                productInfo = indentModel.productInfo,
                Maker = indentModel.Maker,
                IsMakerCompleted = indentModel.IsMakerCompleted,
                MakerStatus = indentModel.MakerStatus,
                // MakerDate= indentModel.MakerDate,
                NameOfTheProject = indentModel.NameOfTheProject,
                IsChecker1Completed = indentModel.IsChecker1Completed,
                Checker1 = indentModel.Checker1,
                Checker1Status = indentModel.Checker1Status,
                Photo = indentModel.Photo,
                PrincipalFlag = indentModel.PrincipalFlag,
                MakerOrHodFlag = indentModel.MakerOrHodFlag,
                currentFileQueueStatus = indentModel.currentFileQueueStatus,
                Note = indentModel.Note,
                listOfRecommendedSupplier1 = indentModel.listOfRecommendedSupplier1,
                listOfRecommendedSupplier2 = indentModel.listOfRecommendedSupplier2,
                listOfRecommendedSupplier3 = indentModel.listOfRecommendedSupplier3,
                listOfRecommendedSupplier4 = indentModel.listOfRecommendedSupplier4,
                CreatedBy = indentModel.CreatedBy,
                CreatedDate = indentModel.CreatedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<IndentModel>> UpdateIndentDetails(IndentModel indentModel)
        {
            //SqlDateTime sqldatenull;
            //sqldatenull = SqlDateTime.Null;           
            var spName = ConstantSPnames.SP_UPDATEINDENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<IndentModel>(spName, new
            {
                refNo = indentModel.RefNo,
                ReferenceNo = indentModel.ReferenceNo,
                FormDate = indentModel.FormDate,
                NameOfTheProgramme = indentModel.NameOfTheProgramme,
                NameOfTheLaboratory = indentModel.NameOfTheLaboratory,
                HeadOfAccount = indentModel.HeadOfAccount,
                purpose = indentModel.purpose,
                nature = indentModel.nature,
                Photo = indentModel.Photo,
                CollegeType = indentModel.CollegeType,
                requirement = indentModel.requirement,
                productInfo = indentModel.productInfo,
                Maker = indentModel.Maker,
                Checker1 = indentModel.Checker1,
                Checker2 = indentModel.Checker2,
                Checker3 = indentModel.Checker3,
                Checker4 = indentModel.Checker4,
                FacultyName = indentModel.FacultyName,
                NameOfTheProject = indentModel.NameOfTheProject,
                IsMakerCompleted = indentModel.IsMakerCompleted,
                IsChecker1Completed = indentModel.IsChecker1Completed,
                IsChecker2Completed = indentModel.IsChecker2Completed,
                IsChecker3Completed = indentModel.IsChecker3Completed,
                IsChecker4Completed = indentModel.IsChecker4Completed,
                PrincipalFlag = indentModel.PrincipalFlag,
                MakerStatus = indentModel.MakerStatus,
                Checker1Status = indentModel.Checker1Status,
                Checker2Status = indentModel.Checker2Status,
                Checker3Status = indentModel.Checker3Status,
                Checker4Status = indentModel.Checker4Status,
                MakerOrHodFlag = indentModel.MakerOrHodFlag,

                //MakerDate = indentModel.MakerDate,
                //Checker1Date = indentModel.Checker1Date,
                //Checker2Date = indentModel.Checker2Date,
                //Checker3Date = indentModel.Checker3Date,
                //Checker4Date = indentModel.Checker4Date,
                currentFileQueueStatus = indentModel.currentFileQueueStatus,
                Note = indentModel.Note,
                listOfRecommendedSupplier1 = indentModel.listOfRecommendedSupplier1,
                listOfRecommendedSupplier2 = indentModel.listOfRecommendedSupplier2,
                listOfRecommendedSupplier3 = indentModel.listOfRecommendedSupplier3,
                listOfRecommendedSupplier4 = indentModel.listOfRecommendedSupplier4,
                ModifiedBy = indentModel.ModifiedBy,
                ModifiedDate = indentModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());

        }
        public Task<List<IndentModel>> UpdateQuatationStatusDetails(QuatationModel quatationModel)
        {
            //SqlDateTime sqldatenull;
            //sqldatenull = SqlDateTime.Null;           
            var spName = ConstantSPnames.SP_UPDATEQUATATIONSTATUS;
            return Task.Factory.StartNew(() => _db.Connection.Query<IndentModel>(spName, new
            {
                RefNo = quatationModel.RefNo,
                PrincipalFlag = quatationModel.principalFlag,
                currentFileQueueStatus = quatationModel.currentFileQueueStatus,
                IsMakerCompleted = quatationModel.IsMakerCompleted,
                IsChecker1Completed = quatationModel.IsChecker1Completed,
                IsChecker2Completed = quatationModel.IsChecker2Completed,
                IsChecker3Completed = quatationModel.IsChecker3Completed,
                IsChecker4Completed = quatationModel.IsChecker4Completed,
                ModifiedBy = quatationModel.ModifiedBy,
                ModifiedDate = quatationModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<IndentModel>> DeleteIndentDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEINDENTFORM;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<IndentModel>(spName, new { RefNo = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }
        public Task<List<FormRoleModel>> GetFormRole(int? id)
        {
            var spName = ConstantSPnames.SP_GETFORMROLE;
            return Task.Factory.StartNew(() => _db.Connection.Query<FormRoleModel>(spName, new { RoleId = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<FormRoleModel>> InsertFormRole(FormRoleModel roleModel)
        {
            var spName = ConstantSPnames.SP_INSERTFORMROLE;
            return Task.Factory.StartNew(() => _db.Connection.Query<FormRoleModel>(spName, new
            {
                RoleName = roleModel.RoleName,
                RolePage = roleModel.RolePage,
                IsActive = roleModel.IsActive,
                // RoleId=roleMaster.RoleId  ,
                CreatedBy = roleModel.CreatedBy,
                CreatedDate = roleModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<FormRoleModel>> UpdateFormRole(FormRoleModel roleModel)
        {
            var spName = ConstantSPnames.SP_UPDATEFORMRole;
            return Task.Factory.StartNew(() => _db.Connection.Query<FormRoleModel>(spName, new
            {
                RoleName = roleModel.RoleName,
                RolePage = roleModel.RolePage,
                IsActive = roleModel.IsActive,
                RoleId = roleModel.RoleId,
                ModifiedBy = roleModel.ModifiedBy,
                ModifiedDate = roleModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<string> DeleteFormRole(int id)
        {
            var spName = ConstantSPnames.SP_DELETEFORMROLE;
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RoleId", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                return Task.Factory.StartNew(() => "Success");                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => ex.Message);
            }
        }
        public string SearchAndReplaceIndentForm(int id)
        {
            string column = string.Empty;
            var spName = ConstantSPnames.SP_GETAllINDENTDETAILS;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
            DataTable ds = new DataTable();
            var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand(spName, myConnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RefNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                using (var da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                }
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == "IndentForm.docx"));
            var row = ds.Rows[0].ItemArray;
            var col = ds.Columns;
            var signPath = Path.Combine(Directory.GetCurrentDirectory(), "Signature");
            var signFiles = Directory.GetFiles(Path.Combine(signPath)).ToList();
            row[2] = String.Format("{0:dd.MM.yyyy}", row[2]);
            row[31] = String.Format("{0:dd.MM.yyyy}", row[31]);
            row[32] = String.Format("{0:dd.MM.yyyy}", row[32]);
            row[33] = String.Format("{0:dd.MM.yyyy}", row[33]);
            row[34] = String.Format("{0:dd.MM.yyyy}", row[34]);
            row[35] = String.Format("{0:dd.MM.yyyy}", row[35]);

            for (int i = 0; i < row.Count(); i++)
            {
                if (i == 15)
                {
                    var items = JsonConvert.DeserializeObject<List<IndentJsonModel>>(row[15].ToString());
                    doc.InsertParagraph();
                    Table t = doc.AddTable(items.Count + 1, 8);
                    t.Alignment = Alignment.center;
                    t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.SetWidths(new float[] { 35, 270, 110, 150, 210, 100, 120 });
                    t.Rows[0].Cells[1].Paragraphs.First().Append("Description of Items/Product").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[2].Paragraphs.First().Append("Qty Required").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[3].Paragraphs.First().Append("Units").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[4].Paragraphs.First().Append("Cost Per Unit").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[5].Paragraphs.First().Append("Total Budget (Approx)").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[6].Paragraphs.First().Append("Budget").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[7].Paragraphs.First().Append("Cum.Expenses").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82)); t.Alignment = Alignment.left;


                    for (int k = 0; k < items.Count; k++)
                    {
                        t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString());
                        t.SetWidths(new float[] { 35, 270, 130, 150, 130, 100, 120 });
                        //{ 35, 300, 120, 130, 120 });
                        t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].descriptionofItems == null ? "" : items[k].descriptionofItems.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].qtyRequired == null ? "" : items[k].qtyRequired.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[3].Paragraphs.First().Append(items[k].units.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[4].Paragraphs.First().Append(items[k].costPerUnit.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[5].Paragraphs.First().Append(((items[k].totalBudget == null) ? "0" : items[k].totalBudget).ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[6].Paragraphs.First().Append(items[k].budget.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[7].Paragraphs.First().Append(items[k].cumulativeExpenses.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    }
                    //doc.InsertTable(t);
                    doc.ReplaceTextWithObject("<Table>", t);
                }
                else
                {
                    var formatcolumn = "<" + col[i] + ">";
                    doc.ReplaceText(formatcolumn, row[i].ToString());
                }
            }
            doc.AddProtection(EditRestrictions.readOnly);
            doc.SaveAs(strfilepath);
            //var docp = new Aspose.Words.Document("Input.docx");
            // docp.Save("Output.pdf");
            return strfilepath;
        }
        public string SearchAndReplaceFdpForm(int id)
        {
            string column = string.Empty;
            var spName = ConstantSPnames.SP_GETAllFDPDETAILS;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
            string savepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.PdfFile.ToString();

            DataTable ds = new DataTable();
            var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand(spName, myConnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RefNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                using (var da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                }
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == "FdpForm.docx"));
            var row = ds.Rows[0].ItemArray;
            var col = ds.Columns;
            var signPath = Path.Combine(Directory.GetCurrentDirectory(), "Signature");
            var signFiles = Directory.GetFiles(Path.Combine(signPath)).ToList();
            row[1] = String.Format("{0:dd.MM.yyyy}", row[1]);
            row[4] = String.Format("{0:dd.MM.yyyy}", row[4]);
            row[5] = String.Format("{0:dd.MM.yyy}", row[5]);
            row[6] = row[6].ToString() + "day(s) [From: " + row[4] + " - To:" + row[5] + "]";

            row[16] = row[16].ToString().ToLower() == "true" ? "Yes" : row[16].ToString().ToLower() == "false" ? "No" : "";
            row[17] = row[17].ToString().ToLower() == "true" ? "Yes" : row[17].ToString().ToLower() == "false" ? "No" : "";
            row[26] = String.Format("{0:dd.MM.yyyy}", row[26]);
            row[31] = String.Format("{0:dd.MM.yyyy}", row[31]);
            row[38] = String.Format("{0:dd.MM.yyyy}", row[38]);

            for (int i = 0; i < row.Count(); i++)
            {
                if (i == 15)
                {
                    var items = JsonConvert.DeserializeObject<List<FdpJsonModel>>(row[15].ToString());
                    doc.InsertParagraph();
                    Table t = doc.AddTable(items.Count + 1, 7);
                    t.Alignment = Alignment.center;
                    t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.SetWidths(new float[] { 35, 85, 70, 70, 100, 80, 80 });
                    t.SetColumnWidth(0, 500);
                    t.Rows[0].Cells[1].Paragraphs.First().Append("Name of Faculty").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Rows[0].Cells[2].Paragraphs.First().Append("Designation").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Rows[0].Cells[3].Paragraphs.First().Append("Department").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Rows[0].Cells[4].Paragraphs.First().Append("Registration Fees").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Rows[0].Cells[5].Paragraphs.First().Append("Appx TA/DA").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Rows[0].Cells[6].Paragraphs.First().Append("Total Budget").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    t.Alignment = Alignment.left;
                    for (int k = 0; k < items.Count; k++)
                    {
                        t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.SetWidths(new float[] { 35, 85, 70, 70, 100, 80, 80 });
                        t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].nameofFaculty == null ? "" : items[k].nameofFaculty.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].designation == null ? "" : items[k].designation.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[3].Paragraphs.First().Append(items[k].departmentName == null ? "" : items[k].departmentName.ToString().Split('-').Last()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[4].Paragraphs.First().Append(items[k].registrationFees.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[5].Paragraphs.First().Append(items[k].appexTAandDA == null ? "" : items[k].appexTAandDA.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[6].Paragraphs.First().Append(items[k].totalBudget.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    }

                    //doc.InsertTable(t);
                    doc.ReplaceTextWithObject("<Table>", t);
                }


                else
                {
                    var formatcolumn = "<" + col[i] + ">";
                    doc.ReplaceText(formatcolumn, row[i].ToString());
                }
            }
            doc.AddProtection(EditRestrictions.readOnly);

            //var app = new Application();
            //var wdoc = app.Documents.Open(strfilepath);
            //wdoc.ExportAsFixedFormat("Report.pdf", WdExportFormat.wdExportFormatPDF);
            //wdoc.Close();
            //app.Quit();
            //var docp = new Aspose.Words.Document(strfilepath);
            //docp.Save("C:\\Data\\Report.pdf");
            doc.SaveAs(strfilepath);
            //var documentp = new Spire.Doc.Document(strfilepath);
            ////Load a sample document
            //documentp.LoadFromFile(strfilepath);
            ////Save to PDF
            //documentp.SaveToFile(savepath);
            return strfilepath;

        }

        public string SearchAndReplaceQuatationForm(int id, int flag)
        {
            string column = string.Empty;
            var spName = ConstantSPnames.SP_GETAllINDENTDETAILS;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
            string savepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.PdfFile.ToString();
            //_appSettings.Settings.FileNames.ToString();
            DataTable ds = new DataTable();
            var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand(spName, myConnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RefNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                using (var da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                }
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            var docName = "";
            docName = (flag == 3) ? "QuotationFormApproved.docx" : "QuotationForm.docx";
            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == docName));
            var row = ds.Rows[0].ItemArray;
            var col = ds.Columns;
            row[2] = DateTime.Now.ToString("dd.MM.yyyy");
            row[35] = String.Format("{0:dd.MM.yyyy}", row[35]);

            for (int i = 0; i < row.Count(); i++)
            {
                if (i == 13)
                {
                    if (row[13].ToString() == "Aided")
                    {
                        row[2] = DateTime.Now.Date.AddDays(15).ToString("dd.MM.yyyy");
                        var item = "on " + row[2] + " 5'o clock";
                        doc.ReplaceText("<Tag>", item);
                    }
                    else
                    {
                        var ssitem = "is immediate";
                        doc.ReplaceText("<Tag>", ssitem);
                    }
                }
                if (i == 15)
                {
                    var items = JsonConvert.DeserializeObject<List<IndentJsonModel>>(row[15].ToString());
                    doc.InsertParagraph();
                    Table t = doc.AddTable(items.Count + 1, 3);
                    t.Alignment = Alignment.center;
                    t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.SetWidths(new float[] { 35, 600, 300 });
                    t.Rows[0].Cells[1].Paragraphs.First().Append("Description of Items/Product").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Rows[0].Cells[2].Paragraphs.First().Append("Qty Required").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                    t.Alignment = Alignment.left;

                    for (int k = 0; k < items.Count; k++)
                    {
                        t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString());
                        t.SetWidths(new float[] { 35, 600, 300 });
                        t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].descriptionofItems == null ? "" : items[k].descriptionofItems.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                        t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].qtyRequired == null ? "" : items[k].qtyRequired.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                    }
                    //doc.InsertTable(t);
                    doc.ReplaceTextWithObject("<Table>", t);
                }
                else
                {
                    var formatcolumn = "<" + col[i] + ">";
                    //row[i] = (row[i] == "") ? " " : row[i];
                    doc.ReplaceText(formatcolumn, row[i].ToString());
                }
            }
            doc.AddProtection(EditRestrictions.readOnly);
            // doc.SaveAs(strfilepath);
            //var docp = new Aspose.Words.Document(strfilepath);
            //docp.Save(strfilepath);

            doc.SaveAs(strfilepath);
            var documentp = new Spire.Doc.Document(strfilepath);
            //Load a sample document
            documentp.LoadFromFile(strfilepath);
            //Save to PDF
            documentp.SaveToFile(savepath);
            return savepath;
        }
        public Task<List<FdpModel>> GetAllFdpDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETAllFDPDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<FdpModel>(spName,
                new { RefNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<FdpModel>> InsertFdpDetails(FdpModel fdpModel)
        {
            // fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;

            var spName = ConstantSPnames.SP_INSERTFDP;
            return Task.Factory.StartNew(() => _db.Connection.Query<FdpModel>(spName, new
            {
                FormDate = fdpModel.FormDate,
                RelievingOrderIssuedon = fdpModel.RelievingOrderIssuedon,
                DepartmentId = fdpModel.DepartmentId,
                TitleOfTheProgramme = fdpModel.TitleOfTheProgramme,
                StartDate = fdpModel.StartDate,
                EndDate = fdpModel.EndDate,
                NoOfDaysAttended = fdpModel.NoOfDaysAttended,
                venue = fdpModel.venue,
                ConductedBy = fdpModel.ConductedBy,
                TypeOfProgramme = fdpModel.TypeOfProgramme,
                DescriptionOfOthers = fdpModel.DescriptionOfOthers,
                level = fdpModel.level,
                HeadOfAccount = fdpModel.HeadOfAccount,
                ProgrammeDetails = fdpModel.ProgrammeDetails,
                collegeType = fdpModel.collegeType,
                FacultyName = fdpModel.FacultyName,
                SubmittedReleivingOrder = fdpModel.SubmittedReleivingOrder,
                SubmittedCopyOfCertificate = fdpModel.SubmittedCopyOfCertificate,
                TaDaBroneBy = fdpModel.TaDaBroneBy,
                Photo = fdpModel.Photo,
                LevelOthers = fdpModel.LevelOthers,
                Maker = fdpModel.Maker,
                IsMakerCompleted = fdpModel.IsMakerCompleted,
                MakerStatus = fdpModel.MakerStatus,
                IsChecker1Completed = fdpModel.IsChecker1Completed,
                Checker1 = fdpModel.Checker1,
                Checker1Status = fdpModel.Checker1Status,
                //MakerDate= fdpModel.MakerDate,
                MakerOrHodFlag = fdpModel.MakerOrHodFlag,
                Note = fdpModel.Note,
                CurrentFileQueueStatus = fdpModel.CurrentFileQueueStatus,
                CreatedBy = fdpModel.CreatedBy,
                CreatedDate = fdpModel.CreatedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<FdpModel>> UpdateFdpDetails(FdpModel fdpModel)
        {
            var spName = ConstantSPnames.SP_UPDATEFDPDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<FdpModel>(spName, new
            {
                refNo = fdpModel.RefNo,
                RelievingOrderIssuedon = fdpModel.RelievingOrderIssuedon,
                FormDate = fdpModel.FormDate,
                DepartmentId = fdpModel.DepartmentId,
                TitleOfTheProgramme = fdpModel.TitleOfTheProgramme,
                StartDate = fdpModel.StartDate,
                EndDate = fdpModel.EndDate,
                NoOfDaysAttended = fdpModel.NoOfDaysAttended,
                ConductedBy = fdpModel.ConductedBy,
                venue = fdpModel.venue,
                TypeOfProgramme = fdpModel.TypeOfProgramme,
                level = fdpModel.level,
                HeadOfAccount = fdpModel.HeadOfAccount,
                Photo = fdpModel.Photo,
                LevelOthers = fdpModel.LevelOthers,
                collegeType = fdpModel.collegeType,
                FacultyName = fdpModel.FacultyName,
                ProgrammeDetails = fdpModel.ProgrammeDetails,
                DescriptionOfOthers = fdpModel.DescriptionOfOthers,
                SubmittedReleivingOrder = fdpModel.SubmittedReleivingOrder,
                SubmittedCopyOfCertificate = fdpModel.SubmittedCopyOfCertificate,
                TaDaBroneBy = fdpModel.TaDaBroneBy,
                Maker = fdpModel.Maker,
                Checker1 = fdpModel.Checker1,
                Checker2 = fdpModel.Checker2,
                Checker3 = fdpModel.Checker3,
                Checker4 = fdpModel.Checker4,
                IsMakerCompleted = fdpModel.IsMakerCompleted,
                IsChecker1Completed = fdpModel.IsChecker1Completed,
                IsChecker2Completed = fdpModel.IsChecker2Completed,
                IsChecker3Completed = fdpModel.IsChecker3Completed,
                IsChecker4Completed = fdpModel.IsChecker4Completed,
                MakerStatus = fdpModel.MakerStatus,
                Checker1Status = fdpModel.Checker1Status,
                Checker2Status = fdpModel.Checker2Status,
                Checker3Status = fdpModel.Checker3Status,
                Checker4Status = fdpModel.Checker4Status,
                //MakerDate = fdpModel.MakerDate,
                //Checker1Date = fdpModel.Checker1Date,
                //Checker2Date = fdpModel.Checker2Date,
                //Checker3Date = fdpModel.Checker3Date,
                //Checker4Date = fdpModel.Checker4Date,
                MakerOrHodFlag = fdpModel.MakerOrHodFlag,
                Note = fdpModel.Note,
                CurrentFileQueueStatus = fdpModel.CurrentFileQueueStatus,
                ModifiedBy = fdpModel.ModifiedBy,
                ModifiedDate = fdpModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());

        }
        public Task<List<FdpModel>> DeleteFdpDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEFDPFORM;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<FdpModel>(spName, new { RefNo = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }
        public Task<List<InventoryIssuedMappingModel>> GetAllInventoryIssuedDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLINVENTORYISSUEDDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryIssuedMappingModel>(spName,
            new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventoryIssuedMappingModel>> InsertInventoryIssuedDetails(InventoryIssuedMappingModel inventoryIssuedMappingModel)
        {
            var spName = ConstantSPnames.SP_INSERTINVENTORYISSUEDDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryIssuedMappingModel>(spName, new
            {
                IssueDate = inventoryIssuedMappingModel.IssueDate,
                InventoryId = inventoryIssuedMappingModel.InventoryId,
                DepartmentId = inventoryIssuedMappingModel.DepartmentId,
                CollegeType = inventoryIssuedMappingModel.CollegeType,
                Store = inventoryIssuedMappingModel.Store,
                Year = inventoryIssuedMappingModel.Year,
                NoOfItems = inventoryIssuedMappingModel.NoOfItems,
                AvailableStock = inventoryIssuedMappingModel.AvailableStock,
                StudentOrFaculty = inventoryIssuedMappingModel.StudentOrFaculty,
                StudentId = inventoryIssuedMappingModel.StudentId,
                FacultyId = inventoryIssuedMappingModel.FacultyId,
                Others = inventoryIssuedMappingModel.Others,
                IssuedBy = inventoryIssuedMappingModel.IssuedBy,
                IssuedQuantity = inventoryIssuedMappingModel.IssuedQuantity,
                //CloseStock = inventoryIssuedMappingModel.CloseStock,
                CreatedBy = inventoryIssuedMappingModel.CreatedBy,
                CreatedDate = inventoryIssuedMappingModel.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventoryIssuedMappingModel>> UpdateInventoryIssuedDetails(InventoryIssuedMappingModel inventoryIssuedMappingModel)
        {
            var spName = ConstantSPnames.SP_UPDATEINVENTORYISSUEDDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryIssuedMappingModel>(spName, new
            {
                Id = inventoryIssuedMappingModel.Id,
                IssueDate = inventoryIssuedMappingModel.IssueDate,
                InventoryId = inventoryIssuedMappingModel.InventoryId,
                DepartmentId = inventoryIssuedMappingModel.DepartmentId,
                CollegeType = inventoryIssuedMappingModel.CollegeType,
                Store = inventoryIssuedMappingModel.Store,
                Year = inventoryIssuedMappingModel.Year,
                NoOfItems = inventoryIssuedMappingModel.NoOfItems,
                AvailableStock = inventoryIssuedMappingModel.AvailableStock,
                StudentOrFaculty = inventoryIssuedMappingModel.StudentOrFaculty,
                StudentId = inventoryIssuedMappingModel.StudentId,
                FacultyId = inventoryIssuedMappingModel.FacultyId,
                Others = inventoryIssuedMappingModel.Others,

                IssuedBy = inventoryIssuedMappingModel.IssuedBy,
                IssuedQuantity = inventoryIssuedMappingModel.IssuedQuantity,
                //CloseStock = inventoryIssuedMappingModel.CloseStock,
                ModifiedBy = inventoryIssuedMappingModel.ModifiedBy,
                ModifiedDate = inventoryIssuedMappingModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());



        }
        public Task<List<InventoryIssuedMappingModel>> DeleteInventoryIssuedDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEINVENTORYISSUED;
            return Task.Factory.StartNew(() =>
            _db.Connection.Query<InventoryIssuedMappingModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
            .ToList());
        }
        public string DownloadStockReport(DateTime StockDate, string? Store)
        {
            try
            {
                var spName = ConstantSPnames.SP_STOCKREPORT;
                var storeName = Store == null ? "all" : Store;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                _appSettings.Settings.FileName.ToString();
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
                DataTable ds = new DataTable();
                //int i = 0;
                //int SNo = 1;
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        objCmd.Parameters.Add("@StockDate", SqlDbType.VarChar).Value = (StockDate).ToString("yyyy-MM-dd");
                        objCmd.Parameters.Add("@Store", SqlDbType.VarChar).Value = Store;
                        da.Fill(ds);
                    }
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    int rowLength = 0;
                    int j = 0;
                    int ColWidth = ds.Columns.Count;
                    var ws = wb.Worksheets.Add("Stock_Report");
                    //var styleAlign3;
                    ws.Cell(++rowLength, 1).Value = "THIAGARAJAR POLYTECHNIC COLLEGE STUDENTS CO-OPERATIVE STORES LTD., SALEM - 636005";
                    // "Stock Report as on (LIST =II with profit of standard 15% of cost price";

                    ws.Range(rowLength, 1, ++rowLength, ds.Columns.Count).Merge().AddToNamed("Titles");
                    ws.Cell(++rowLength, 1).Value = "Cumulative Stock Report as on  " + storeName + " (LIST =II with profit of standard 15% of cost price)";
                    ws.Range(rowLength, 1, ++rowLength, ds.Columns.Count).Merge().AddToNamed("Titles");
                    rowLength++;
                    for (j = 0; j < ds.Columns.Count; j++)
                    {
                        var tableHeader = ws.Cell(rowLength, j + 1).Value = ds.Columns[j].ColumnName;

                    }
                    ws.Range(rowLength, 1, rowLength, j).AddToNamed("Titles");
                    var rangeWithData = ws.Cell(++rowLength, 1).InsertData(ds);
                    rangeWithData.Style.Font.SetFontSize(9);
                    rowLength = rowLength - 1 + ds.Rows.Count;
                    var titlesStyle = wb.Style;
                    titlesStyle.Font.Bold = true;
                    titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Alignment.WrapText = true;
                    titlesStyle.Font.SetFontSize(10);
                    //styleAlign3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    // wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                    var namedRange = wb.NamedRanges.NamedRange("Titles");

                    if (namedRange != null)
                    {
                        namedRange.Ranges.Style = titlesStyle;
                    }
                    else
                    {
                        Console.WriteLine("Named range 'Titles' not found.");
                    }
                    IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(rowLength, ColWidth).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Columns().AdjustToContents();
                    if (File.Exists(strfilepath))
                    {
                        File.Delete(strfilepath);
                    }
                    //lblerror.Text = "three";
                    wb.SaveAs(strfilepath);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                return strfilepath;
            }
            catch (Exception ex)
            {
                return null;
            }

            /*var ws = wb.Worksheets.Add("StockReport");
            ws.Cell(1, 1).Value = "THIYAGARAJA POLYTECHNIC COLLEGE STUDENTS CO-OPERATIVE STORES LTD., SALEM - 636005";
            //ws.Range(1, 1, 1, i).Merge().AddToNamed("Titles");
            ws.Cell(2, 1).Value =
                "Stock Report as on (LIST =II with profit of standard 15% of cost price";*/
        }
        public Task<List<InventoryModel>> GetInventory(int? id)
        {
            var spName = ConstantSPnames.SP_GETINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventoryModel>> InsertInventory(InventoryModel inventoryModel)
        {
            var spName = ConstantSPnames.SP_INSERTINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryModel>(spName, new
            {
                Name = inventoryModel.Name,
                Store = inventoryModel.Store,

                IsActive = inventoryModel.IsActive,
                // RoleId=roleMaster.RoleId  ,
                CreatedBy = inventoryModel.CreatedBy,
                CreatedDate = inventoryModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventoryModel>> UpdateInventory(InventoryModel inventoryModel)
        {
            var spName = ConstantSPnames.SP_UPDATEINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventoryModel>(spName, new
            {
                Name = inventoryModel.Name,
                Store = inventoryModel.Store,

                IsActive = inventoryModel.IsActive,
                Id = inventoryModel.Id,
                ModifiedBy = inventoryModel.ModifiedBy,
                ModifiedDate = inventoryModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public string DeleteInventory(int id)
        {

            try
            {
                var spName = ConstantSPnames.SP_DELETEINVENTORY;
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("Id", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                return "Success";                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public Task<List<InventorySpecModel>> GetInventorySpec(int? id)
        {
            var spName = ConstantSPnames.SP_GETINVENTORYSPEC;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventorySpecModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventorySpecModel>> InsertInventorySpec(InventorySpecModel inventorySpecModel)
        {
            var spName = ConstantSPnames.SP_INSERTINVENTORYSPEC;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventorySpecModel>(spName, new
            {
                InventoryId = inventorySpecModel.InventoryId,
                //Store = inventorySpecModel.Store,
                Description = inventorySpecModel.Description,
                Specification = inventorySpecModel.Specification,
                IsActive = inventorySpecModel.IsActive,
                // RoleId=roleMaster.RoleId  ,
                CreatedBy = inventorySpecModel.CreatedBy,
                CreatedDate = inventorySpecModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InventorySpecModel>> UpdateInventorySpec(InventorySpecModel inventorySpecModel)
        {
            var spName = ConstantSPnames.SP_UPDATEINVENTORYSPEC;
            return Task.Factory.StartNew(() => _db.Connection.Query<InventorySpecModel>(spName, new
            {
                InventoryId = inventorySpecModel.InventoryId,
                //Store = inventorySpecModel.Store,
                Description = inventorySpecModel.Description,
                Specification = inventorySpecModel.Specification,
                IsActive = inventorySpecModel.IsActive,
                Id = inventorySpecModel.Id,
                ModifiedBy = inventorySpecModel.ModifiedBy,
                ModifiedDate = inventorySpecModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public string DeleteInventorySpec(int id)
        {
            var spName = ConstantSPnames.SP_DELETEINVENTORYSPEC;
            //return Task.Factory.StartNew(() =>
            //    _db.Connection.Query<InventorySpecModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
            //        .ToList());
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("Id", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                return "Success";                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Task<List<StockInventoryModel>> GetStockInventory(int? id)
        {
            var spName = ConstantSPnames.SP_GETSTOCKINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<StockInventoryModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<StockInventoryModel>> InsertStockInventory(StockInventoryModel stockInventoryModel)
        {
            var spName = ConstantSPnames.SP_INSERTSTOCKINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<StockInventoryModel>(spName, new
            {
                InventoryId = stockInventoryModel.InventoryId,
                StockDate = stockInventoryModel.StockDate,
                Store = stockInventoryModel.Store,
                AvailableQuantity = stockInventoryModel.AvailableQuantity,
                /*PurchasedBy = stockInventoryModel.PurchasedBy,
                RequestedBy = stockInventoryModel.RequestedBy,
                IndentQuantity = stockInventoryModel.IndentQuantity,
                PrintedQuantity = stockInventoryModel.PrintedQuantity,
                ReceivedQuantity = stockInventoryModel.ReceivedQuantity,
                VendarDetails = stockInventoryModel.VendarDetails,
                UnitPrize = stockInventoryModel.UnitPrize,
                TotalPrize = stockInventoryModel.TotalPrize,
                Discount = stockInventoryModel.Discount,*/
                CreatedBy = stockInventoryModel.CreatedBy,
                CreatedDate = stockInventoryModel.CreatedDate,



            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<StockInventoryModel>> UpdateStockInventory(StockInventoryModel stockInventoryModel)
        {
            var spName = ConstantSPnames.SP_UPDATESTOCKINVENTORY;
            return Task.Factory.StartNew(() => _db.Connection.Query<StockInventoryModel>(spName, new
            {
                Id = stockInventoryModel.Id,
                InventoryId = stockInventoryModel.InventoryId,
                StockDate = stockInventoryModel.StockDate,
                Store = stockInventoryModel.Store,
                AvailableQuantity = stockInventoryModel.AvailableQuantity,

                ModifiedBy = stockInventoryModel.ModifiedBy,
                ModifiedDate = stockInventoryModel.ModifiedDate,

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<StockInventoryModel>> DeleteStockInventory(int id)
        {
            var spName = ConstantSPnames.SP_DELETESTOCKINVENTORY;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<StockInventoryModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }
        public Task<List<LabDetailsModel>> GetAllLabDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLLABDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<LabDetailsModel>(spName,
            new { SINo = id }, commandType: CommandType.StoredProcedure).ToList());
            //try
            //{                
            //    using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
            //    sqlconnection.Open();
            //    SqlCommand command = new SqlCommand(spName, sqlconnection);
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.Parameters.Add("SINo", SqlDbType.Int).Value = id;
            //    command.ExecuteNonQuery();
            //    return "Success";                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
        }
        public Task<List<LabDetailsModel>> InsertLabDetails(LabDetailsModel labDetailsModel)
        {
            var spName = ConstantSPnames.SP_INSERTLABDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<LabDetailsModel>(spName, new
            {
                CollegeType = labDetailsModel.CollegeType,
                DepartmentId = labDetailsModel.DepartmentId,
                LabName = labDetailsModel.LabName,
                CreatedBy = labDetailsModel.CreatedBy,
                CreatedDate = labDetailsModel.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<LabDetailsModel>> UpdateLabDetails(LabDetailsModel labDetailsModel)
        {
            var spName = ConstantSPnames.SP_UPDATELABDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<LabDetailsModel>(spName, new
            {
                SINo = labDetailsModel.SINo,
                CollegeType = labDetailsModel.CollegeType,
                DepartmentId = labDetailsModel.DepartmentId,
                LabName = labDetailsModel.LabName,
                ModifiedBy = labDetailsModel.ModifiedBy,
                ModifiedDate = labDetailsModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());



        }
        public string DeleteLabDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETELABDETAILS;
            //return Task.Factory.StartNew(() =>
            //_db.Connection.Query<LabDetailsModel>(spName, new { SINo = id }, commandType: CommandType.StoredProcedure)
            //.ToList());
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("SINo", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                return "Success";                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return ex.Message;
                //}
            }
        }

        public Task<List<HOADetailsModel>> GetAllHOADetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLHOADETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<HOADetailsModel>(spName,
            new { SINo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<HOADetailsModel>> InsertHOADetails(HOADetailsModel hoaDetailsModel)
        {
            var spName = ConstantSPnames.SP_INSERTHOADETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<HOADetailsModel>(spName, new
            {
                CollegeType = hoaDetailsModel.CollegeType,
                HeadOfAccount = hoaDetailsModel.HeadOfAccount,
                IsActiveFdp = hoaDetailsModel.IsActiveFdp,
                CreatedBy = hoaDetailsModel.CreatedBy,
                CreatedDate = hoaDetailsModel.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<HOADetailsModel>> UpdateHOADetails(HOADetailsModel hoaDetailsModel)
        {
            var spName = ConstantSPnames.SP_UPDATEHOADETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<HOADetailsModel>(spName, new
            {
                SINo = hoaDetailsModel.SINo,
                CollegeType = hoaDetailsModel.CollegeType,
                HeadOfAccount = hoaDetailsModel.HeadOfAccount,
                IsActiveFdp = hoaDetailsModel.IsActiveFdp,
                ModifiedBy = hoaDetailsModel.ModifiedBy,
                ModifiedDate = hoaDetailsModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());



        }
        public string DeleteHOADetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEHOADETAILS;
            //return Task.Factory.StartNew(() =>
            //_db.Connection.Query<HOADetailsModel>(spName, new { SINo = id }, commandType: CommandType.StoredProcedure)
            //.ToList());
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("Id", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                return "Success";                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Task<List<PurchasedOrderModel>> GetPurchasedOrder(int? id)
        {
            var spName = ConstantSPnames.SP_GETPURCHASEDORDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<PurchasedOrderModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<PurchasedOrderModel>> InsertPurchasedOrder(PurchasedOrderModel purchasedOrderModel)
        {
            var spName = ConstantSPnames.SP_INSERTPURCHASEDORDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<PurchasedOrderModel>(spName, new
            {
                InventoryId = purchasedOrderModel.InventoryId,
                PurchasedDate = purchasedOrderModel.PurchasedDate,
                // PurchasedBy = stockInventoryModel.PurchasedBy,
                RequestedBy = purchasedOrderModel.RequestedBy,
                Store = purchasedOrderModel.Store,
                HeadOfAccount = purchasedOrderModel.HeadOfAccount,
                //AvailableQuantity = stockInventoryModel.AvailableQuantity,
                DepartmentId = purchasedOrderModel.DepartmentId,
                CollegeType = purchasedOrderModel.CollegeType,
                IndentQuantity = purchasedOrderModel.IndentQuantity,
                PrintedQuantity = purchasedOrderModel.PrintedQuantity,
                ReceivedQuantity = purchasedOrderModel.ReceivedQuantity,
                VendarDetails = purchasedOrderModel.VendarDetails,
                UnitPrize = purchasedOrderModel.UnitPrize,
                TotalPrize = purchasedOrderModel.TotalPrize,
                Discount = purchasedOrderModel.Discount,
                UpdatedStock = purchasedOrderModel.UpdatedStock,
                CreatedBy = purchasedOrderModel.CreatedBy,
                CreatedDate = purchasedOrderModel.CreatedDate,



            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<PurchasedOrderModel>> UpdatePurchasedOrder(PurchasedOrderModel purchasedOrderModel)
        {
            var spName = ConstantSPnames.SP_UPDATEPURCHASEDORDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<PurchasedOrderModel>(spName, new
            {
                Id = purchasedOrderModel.Id,
                InventoryId = purchasedOrderModel.InventoryId,
                PurchasedDate = purchasedOrderModel.PurchasedDate,
                HeadOfAccount = purchasedOrderModel.HeadOfAccount,

                // PurchasedBy = stockInventoryModel.PurchasedBy,
                RequestedBy = purchasedOrderModel.RequestedBy,
                Store = purchasedOrderModel.Store,
                DepartmentId = purchasedOrderModel.DepartmentId,
                CollegeType = purchasedOrderModel.CollegeType,
                // AvailableQuantity = stockInventoryModel.AvailableQuantity,
                IndentQuantity = purchasedOrderModel.IndentQuantity,
                PrintedQuantity = purchasedOrderModel.PrintedQuantity,
                ReceivedQuantity = purchasedOrderModel.ReceivedQuantity,
                VendarDetails = purchasedOrderModel.VendarDetails,
                UnitPrize = purchasedOrderModel.UnitPrize,
                TotalPrize = purchasedOrderModel.TotalPrize,
                UpdatedPrize = purchasedOrderModel.UpdatedPrize,
                Discount = purchasedOrderModel.Discount,
                UpdatedStock = purchasedOrderModel.UpdatedStock,
                ModifiedBy = purchasedOrderModel.ModifiedBy,
                ModifiedDate = purchasedOrderModel.ModifiedDate,

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<PurchasedOrderModel>> DeletePurchasedOrder(int id)
        {
            var spName = ConstantSPnames.SP_DELETEPURCHASEDORDER;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<PurchasedOrderModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }


        public Task<List<BudgetLineModel>> GetBudgetLine(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLBUDGETLINE;
            return Task.Factory.StartNew(() => _db.Connection.Query<BudgetLineModel>(spName, new { SNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BudgetLineModel>> InsertBudgetLine(BudgetLineModel budgetLineModel)
        {
            var spName = ConstantSPnames.SP_INSERTBUDGETLINE;
            return Task.Factory.StartNew(() => _db.Connection.Query<BudgetLineModel>(spName, new
            {
                BudgetDate = budgetLineModel.BudgetDate,
                collegeType = budgetLineModel.collegeType,
                DepartmentId = budgetLineModel.DepartmentId,
                BudgetAmt = budgetLineModel.BudgetAmt,
                HeadOfAccount = budgetLineModel.HeadOfAccount,
                //UpdatedAmt = budgetLineModel.UpdatedAmt,
                CreatedBy = budgetLineModel.CreatedBy,
                CreatedDate = budgetLineModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BudgetLineModel>> UpdateBudgetLine(BudgetLineModel budgetLineModel)
        {
            var spName = ConstantSPnames.SP_UPDATEBUDGETLINE;
            return Task.Factory.StartNew(() => _db.Connection.Query<BudgetLineModel>(spName, new
            {
                SNo = budgetLineModel.SNo,
                BudgetDate = budgetLineModel.BudgetDate,
                collegeType = budgetLineModel.collegeType,
                DepartmentId = budgetLineModel.DepartmentId,
                BudgetAmt = budgetLineModel.BudgetAmt,
                OldAmt = budgetLineModel.OldAmt,
                HeadOfAccount = budgetLineModel.HeadOfAccount,

                //UpdatedAmt = budgetLineModel.UpdatedAmt,
                ModifiedBy = budgetLineModel.ModifiedBy,
                ModifiedDate = budgetLineModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<string> DeleteBudgetLine(int id)
        {
            var spName = ConstantSPnames.SP_DELETEBUDGETLINE;
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("SNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                return Task.Factory.StartNew(() => "Success");                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => ex.Message);
            }
        }
        public Task<List<BudgetHeadModel>> GetBudgetHead(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLBUDGETHEAD;
            return Task.Factory.StartNew(() => _db.Connection.Query<BudgetHeadModel>(spName, new { SNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BudgetReallocateModel>> ReallocateBudgetDetails(BudgetReallocateModel budgetReallocateModel)
        {
            var spName = ConstantSPnames.SP_REALLOCATEBUDGET;
            return Task.Factory.StartNew(() => _db.Connection.Query<BudgetReallocateModel>(spName, new
            {
                SNoFrom = budgetReallocateModel.SNoFrom,
                SNoTo = budgetReallocateModel.SNoTo,
                budgetDate = budgetReallocateModel.budgetDate,
                //departmentIdFrom = budgetReallocateModel.departmentIdFrom,
                departmentIdTo = budgetReallocateModel.departmentIdTo,
                collegeTypeFrom = budgetReallocateModel.collegeTypeFrom,
                //HeadOfAccountFrom =budgetReallocateModel.HeadOfAccountFrom,
                HeadOfAccountTo = budgetReallocateModel.HeadOfAccountTo,
                BudgetAmtFrom = budgetReallocateModel.BudgetAmtFrom,
                BudgetAmtTo = budgetReallocateModel.BudgetAmtTo,
                ModifiedBy = budgetReallocateModel.ModifiedBy,
                ModifiedDate = budgetReallocateModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public string DownloadCumulativeReport(string? Store)
        {
            try
            {
                var spName = ConstantSPnames.SP_STOCKREPORTCUM;
                var storeName = Store == null ? "all" : Store;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                _appSettings.Settings.FileName.ToString();
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
                DataTable ds = new DataTable();
                //int i = 0;
                //int SNo = 1;
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        //objCmd.Parameters.Add("@StockDate", SqlDbType.VarChar).Value = (StockDate).ToString("yyyy-MM-dd");
                        objCmd.Parameters.Add("@Store", SqlDbType.VarChar).Value = Store;
                        da.Fill(ds);
                    }
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    int rowLength = 0;
                    int j = 0;
                    int ColWidth = ds.Columns.Count;
                    var ws = wb.Worksheets.Add("Stock_Report");
                    //var styleAlign3;
                    ws.Cell(++rowLength, 1).Value = "THIAGARAJAR POLYTECHNIC COLLEGE STUDENTS CO-OPERATIVE STORES LTD., SALEM - 636005";
                    /*+ " " +
                        "Stock Report as on (LIST =II with profit of standard 15% of cost price";*/

                    ws.Range(rowLength, 1, ++rowLength, ds.Columns.Count).Merge().AddToNamed("Titles");
                    ws.Cell(++rowLength, 1).Value = "Stock Report as on  " + storeName + " (LIST =II with profit of standard 15% of cost price)";

                    ws.Range(rowLength, 1, ++rowLength, ds.Columns.Count).Merge().AddToNamed("Titles");
                    rowLength++;

                    for (j = 0; j < ds.Columns.Count; j++)
                    {
                        var tableHeader = ws.Cell(rowLength, j + 1).Value = ds.Columns[j].ColumnName;
                    }
                    ws.Range(rowLength, 1, rowLength, j).AddToNamed("Titles");
                    var rangeWithData = ws.Cell(++rowLength, 1).InsertData(ds);
                    rangeWithData.Style.Font.SetFontSize(9);
                    rowLength = rowLength - 1 + ds.Rows.Count;
                    var titlesStyle = wb.Style;
                    titlesStyle.Font.Bold = true;
                    titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                    titlesStyle.Alignment.WrapText = true;
                    titlesStyle.Font.SetFontSize(10);
                    //styleAlign3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                    var namedRange = wb.NamedRanges.NamedRange("Titles");

                    if (namedRange != null)
                    {
                        namedRange.Ranges.Style = titlesStyle;
                    }
                    else
                    {
                        Console.WriteLine("Named range 'Titles' not found.");
                    }
                    IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(rowLength, ColWidth).Address);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Columns().AdjustToContents();
                    if (File.Exists(strfilepath))
                    {
                        File.Delete(strfilepath);
                    }
                    //lblerror.Text = "three";

                    wb.SaveAs(strfilepath);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                return strfilepath;

            }
            catch (Exception ex)
            {
                return null;
            }
            /*var ws = wb.Worksheets.Add("StockReport");
            ws.Cell(1, 1).Value = "THIYAGARAJA POLYTECHNIC COLLEGE STUDENTS CO-OPERATIVE STORES LTD., SALEM - 636005";
            //ws.Range(1, 1, 1, i).Merge().AddToNamed("Titles");
            ws.Cell(2, 1).Value =
                "Stock Report as on (LIST =II with profit of standard 15% of cost price";*/
        }
        public Task<List<OdpModel>> GetAllOdpDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETAllODPDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<OdpModel>(spName,
                new { RefNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<OdpModel>> InsertOdpDetails(OdpModel oDPModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTODPDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<OdpModel>(spName, new
            {
                FormDate = oDPModel.FormDate,
                CollegeType = oDPModel.CollegeType,
                ProgrammeName = oDPModel.ProgrammeName,
                ExamSession = oDPModel.ExamSession,
                DoteType = oDPModel.DoteType,
                DoteReference = oDPModel.DoteReference,
                ExaminersInfo = oDPModel.ExaminersInfo,
                NameOfExam = oDPModel.NameOfExam,
                NoOfDays = oDPModel.NoOfDays,
                FromDate = oDPModel.FromDate,
                ToDate = oDPModel.ToDate,
                DiscriptionOfOthers = oDPModel.DiscriptionOfOthers,
                Note = oDPModel.Note,
                Photo = oDPModel.Photo,
                Maker = oDPModel.Maker,
                IsMakerCompleted = oDPModel.IsMakerCompleted,
                MakerStatus = oDPModel.MakerStatus,
                ReleivingOrderIssuedOn = oDPModel.ReleivingOrderIssuedOn,
                /*IsChecker1Completed = oDPModel.IsChecker1Completed,
                Checker1 = oDPModel.Checker1,
                Checker1Status = oDPModel.Checker1Status,*/
                CurrentFileQueueStatus = oDPModel.CurrentFileQueueStatus,
                CreatedBy = oDPModel.CreatedBy,
                CreatedDate = oDPModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<OdpModel>> UpdateOdpDetails(OdpModel oDPModel)
        {
            var spName = ConstantSPnames.SP_UPDATEODPDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<OdpModel>(spName, new
            {
                RefNo = oDPModel.RefNo,
                FormDate = oDPModel.FormDate,
                CollegeType = oDPModel.CollegeType,
                ProgrammeName = oDPModel.ProgrammeName,
                ExamSession = oDPModel.ExamSession,
                DoteType = oDPModel.DoteType,
                DoteReference = oDPModel.DoteReference,
                ExaminersInfo = oDPModel.ExaminersInfo,
                NoOfDays = oDPModel.NoOfDays,
                FromDate = oDPModel.FromDate,
                ToDate = oDPModel.ToDate,
                DiscriptionOfOthers = oDPModel.DiscriptionOfOthers,
                NameOfExam = oDPModel.NameOfExam,
                Note = oDPModel.Note,
                Photo = oDPModel.Photo,
                Maker = oDPModel.Maker,
                Checker1 = oDPModel.Checker1,
                Checker2 = oDPModel.Checker2,
                IsMakerCompleted = oDPModel.IsMakerCompleted,
                IsChecker1Completed = oDPModel.IsChecker1Completed,
                IsChecker2Completed = oDPModel.IsChecker2Completed,
                MakerStatus = oDPModel.MakerStatus,
                Checker1Status = oDPModel.Checker1Status,
                Checker2Status = oDPModel.Checker2Status,
                CurrentFileQueueStatus = oDPModel.CurrentFileQueueStatus,
                ReleivingOrderIssuedOn = oDPModel.ReleivingOrderIssuedOn,
                ReleivingOrderNo = oDPModel.ReleivingOrderNo,
                ModifiedBy = oDPModel.ModifiedBy,
                ModifiedDate = oDPModel.ModifiedDate
            },
         commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<OdpModel>> DeleteOdpDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEODPFORM;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<OdpModel>(spName, new { RefNo = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<BpeModel>> GetAllBpeDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETAllBPEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<BpeModel>(spName,
                new { RefNo = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BpeModel>> InsertBpeDetails(BpeModel bpeModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTBPE;
            return Task.Factory.StartNew(() => _db.Connection.Query<BpeModel>(spName, new
            {
                //RefNo = bpeModel.RefNo,
                CollegeType = bpeModel.CollegeType,
                ReleivingOrderNo = bpeModel.ReleivingOrderNo,
                DOTEReference = bpeModel.DOTEReference,
                FormDate = bpeModel.FormDate,
                LrNo = bpeModel.LrNo,
                ExamStartDate = bpeModel.ExamStartDate,
                ExamEndDate = bpeModel.ExamEndDate,
                NoOfDays = bpeModel.NoOfDays,
                AppointedAs = bpeModel.AppointedAs,
                AppointedOthers = bpeModel.AppointedOthers,
                NameOfThePractical = bpeModel.NameOfThePractical,
                ExamSession = bpeModel.ExamSession,
                ExaminersInfo = bpeModel.ExaminersInfo,
                Maker = bpeModel.Maker,
                IsMakerCompleted = bpeModel.IsMakerCompleted,
                MakerStatus = bpeModel.MakerStatus,
                Checker1 = bpeModel.Checker1,
                IsChecker1Completed = bpeModel.IsChecker1Completed,
                Checker1Status = bpeModel.Checker1Status,
                CurrentFileQueueStatus = bpeModel.CurrentFileQueueStatus,
                Note = bpeModel.Note,
                CreatedBy = bpeModel.CreatedBy,
                CreatedDate = bpeModel.CreatedDate,
                Photo = bpeModel.Photo,
                RelievingOrderIssuedon = bpeModel.RelievingOrderIssuedon,
                MakerOrHodFlag = bpeModel.MakerOrHodFlag,
                ProgrammeName = bpeModel.ProgrammeName,

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BpeModel>> UpdateBpeDetails(BpeModel bpeModel)
        {
            var spName = ConstantSPnames.SP_UPDATEBPEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<BpeModel>(spName, new
            {
                RefNo = bpeModel.RefNo,
                CollegeType = bpeModel.CollegeType,
                DOTEReference = bpeModel.DOTEReference,
                ReleivingOrderNo = bpeModel.ReleivingOrderNo,
                FormDate = bpeModel.FormDate,
                LrNo = bpeModel.LrNo,
                ExamStartDate = bpeModel.ExamStartDate,
                ExamEndDate = bpeModel.ExamEndDate,
                NoOfDays = bpeModel.NoOfDays,
                AppointedAs = bpeModel.AppointedAs,
                AppointedOthers = bpeModel.AppointedOthers,
                NameOfThePractical = bpeModel.NameOfThePractical,
                ExamSession = bpeModel.ExamSession,
                ExaminersInfo = bpeModel.ExaminersInfo,
                Maker = bpeModel.Maker,
                Checker1 = bpeModel.Checker1,
                Checker2 = bpeModel.Checker2,
                Checker3 = bpeModel.Checker3,
                Checker4 = bpeModel.Checker4,
                IsMakerCompleted = bpeModel.IsMakerCompleted,
                IsChecker1Completed = bpeModel.IsChecker1Completed,
                IsChecker2Completed = bpeModel.IsChecker2Completed,
                IsChecker3Completed = bpeModel.IsChecker3Completed,
                IsChecker4Completed = bpeModel.IsChecker4Completed,
                MakerStatus = bpeModel.MakerStatus,
                Checker1Status = bpeModel.Checker1Status,
                Checker2Status = bpeModel.Checker2Status,
                Checker3Status = bpeModel.Checker3Status,
                Checker4Status = bpeModel.Checker4Status,
                CurrentFileQueueStatus = bpeModel.CurrentFileQueueStatus,
                Note = bpeModel.Note,
                ModifiedBy = bpeModel.ModifiedBy,
                ModifiedDate = bpeModel.ModifiedDate,
                Photo = bpeModel.Photo,
                RelievingOrderIssuedon = bpeModel.RelievingOrderIssuedon,
                MakerOrHodFlag = bpeModel.MakerOrHodFlag,
                ProgrammeName = bpeModel.ProgrammeName,

            },
         commandType: CommandType.StoredProcedure).ToList());



        }
        public Task<List<BpeModel>> DeleteBpeDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEBPEFORM;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<BpeModel>(spName, new { RefNo = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public string SearchAndReplaceBpeForm(int id)
        {
            string column = string.Empty;
            var spName = ConstantSPnames.SP_GETAllBPEDETAILS;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
            string savepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.PdfFile.ToString();



            DataTable ds = new DataTable();
            var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand(spName, myConnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RefNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                using (var da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                }
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            var row = ds.Rows[0].ItemArray;
            var col = ds.Columns;
            var items = JsonConvert.DeserializeObject<List<BpeJsonModel>>(row[13].ToString());
            var FileName = (items.Count == 1) ? "BpeFormSingle.docx" : "BpeFormMultiple.docx";

            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == FileName));

            row[4] = String.Format("{0:dd.MM.yyyy}", row[4]);
            row[6] = String.Format("{0:dd.MM.yyyy}", row[6]);
            row[7] = String.Format("{0:dd.MM.yyy}", row[7]);
            row[43] = String.Format("{0:dd.MM.yyy}", row[43]);

            for (int i = 0; i < row.Count(); i++)
            {
                if (i == 13)
                {
                    if (items.Count == 1)
                    {
                        doc.InsertParagraph();
                        doc.ReplaceText("<FacultyName>", items[0].nameofFaculty == null ? "" : items[0].nameofFaculty.ToString());
                        doc.ReplaceText("<DOTEStaffId>", items[0].doteStaffId == null ? "" : items[0].doteStaffId.ToString());
                        doc.ReplaceText("<Designation>", items[0].designation == null ? "" : items[0].designation.ToString());
                        doc.ReplaceText("<DepartmentName>", items[0].departmentName == null ? "" : items[0].departmentName.ToString().Split('-').Last());
                        doc.ReplaceText("<CollegeCode&Name>", items[0].collegeCodeandName == null ? "" : items[0].collegeCodeandName.ToString());
                        doc.ReplaceText("<CollegeAddress>", items[0].collegeAddress == null ? "" : items[0].collegeAddress.ToString());
                        doc.ReplaceText("<DutyAs>", items[0].dutyAs == null ? "" : items[0].dutyAs.ToString());
                    }
                    else
                    {
                        doc.InsertParagraph();
                        Table t = doc.AddTable(items.Count + 1, 8);
                        t.Alignment = Alignment.center;
                        t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.SetWidths(new float[] { 35, 170, 110, 150, 230, 200 });
                        t.Rows[0].Cells[1].Paragraphs.First().Append("Name of Faculty").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[2].Paragraphs.First().Append("DOTE Staff ID").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[3].Paragraphs.First().Append("Designation").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[4].Paragraphs.First().Append("Department").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[5].Paragraphs.First().Append("College Code & Name").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[6].Paragraphs.First().Append("College Address").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                        t.Rows[0].Cells[7].Paragraphs.First().Append("Duty As").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));

                        for (int k = 0; k < items.Count; k++)
                        {
                            t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.SetWidths(new float[] { 35, 170, 130, 150, 150, 180 });
                            //{ 35, 300, 120, 130, 120 });
                            t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].nameofFaculty == null ? "" : items[k].nameofFaculty.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].doteStaffId == null ? "" : items[k].doteStaffId.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[3].Paragraphs.First().Append(items[k].designation == null ? "" : items[k].designation.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[4].Paragraphs.First().Append(items[k].departmentName == null ? "" : items[k].departmentName.ToString().Split('-').Last()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[5].Paragraphs.First().Append(items[k].collegeCodeandName == null ? "" : items[k].collegeCodeandName.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[6].Paragraphs.First().Append(items[k].collegeAddress == null ? "" : items[k].collegeAddress.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            t.Rows[k + 1].Cells[7].Paragraphs.First().Append(items[k].dutyAs == null ? "" : items[k].dutyAs.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));

                        }
                        //doc.InsertTable(t);
                        doc.ReplaceTextWithObject("<Table>", t);
                    }
                }

                else
                {
                    var formatcolumn = "<" + col[i] + ">";
                    doc.ReplaceText(formatcolumn, row[i].ToString());
                }
            }
            doc.AddProtection(EditRestrictions.readOnly);
            doc.SaveAs(strfilepath);
            return strfilepath;
        }
        public string SearchAndReplaceOdpForm(int id)
        {
            string column = string.Empty;
            var spName = ConstantSPnames.SP_GETAllODPDETAILS;
            string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
            string savepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.PdfFile.ToString();



            DataTable ds = new DataTable();
            var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
            using (SqlConnection myConnection = new SqlConnection(con))
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand(spName, myConnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("RefNo", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                using (var da = new SqlDataAdapter(command))
                {
                    da.Fill(ds);
                }
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            var row = ds.Rows[0].ItemArray;
            var col = ds.Columns;
            row[1] = String.Format("{0:dd.MM.yyyy}", row[1]);
            row[9] = String.Format("{0:dd.MM.yyyy}", row[9]);
            row[10] = String.Format("{0:dd.MM.yyy}", row[10]);
            row[26] = String.Format("{0:dd.MM.yyy}", row[26]);
            var FileName = row[5].ToString() == "Theory Exam" ? "ODPTheoryForm" : row[5].ToString() == "Central Valuation/Revaluation" ? "ODPFormValuation" : "ODPFormOthers";
            var items = JsonConvert.DeserializeObject<List<OdpJsonModel>>(row[8].ToString());
            FileName = items[0].dutyAs == "Chief Supdt." ? FileName + "Chief" : items[0].dutyAs == "Hall Supdt." ? FileName + "Hall" : items[0].dutyAs == "Addl.Chief Supdt." ? FileName + "AddChief" : FileName;

            FileName = items.Count == 1 ? FileName + "Single.docx" : FileName + "Multiple.docx";

            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == FileName));

            for (int i = 0; i < row.Count(); i++)
            {
                if (i == 8)
                {
                    if (items.Count == 1)
                    {
                        if ((row[5].ToString() == "Theory Exam") || (row[5].ToString() == "Other Exams/Activities"))
                        {
                            doc.InsertParagraph();
                            doc.ReplaceText("<FacultyName>", items[0].nameofFaculty == null ? "" : items[0].nameofFaculty.ToString());
                            doc.ReplaceText("<DOTEStaffId>", items[0].doteStaffId == null ? "" : items[0].doteStaffId.ToString());
                            doc.ReplaceText("<Designation>", items[0].designation == null ? "" : items[0].designation.ToString());
                            doc.ReplaceText("<DepartmentName>", items[0].departmentName == null ? "" : items[0].departmentName.ToString().Split('-').Last());
                            doc.ReplaceText("<DutyAs>", items[0].dutyAs == null ? "" : items[0].dutyAs.ToString());
                            doc.ReplaceText("<CollegeCode&Name>", items[0].collegeCodeandName == null ? "" : items[0].collegeCodeandName.ToString());
                            doc.ReplaceText("<CollegeAddress>", items[0].collegeAddress == null ? "" : items[0].collegeAddress.ToString());
                        }
                        else if (row[5].ToString() == "Central Valuation/Revaluation")
                        {
                            doc.InsertParagraph();
                            doc.ReplaceText("<FacultyName>", items[0].nameofFaculty == null ? "" : items[0].nameofFaculty.ToString());
                            doc.ReplaceText("<DOTEStaffId>", items[0].doteStaffId == null ? "" : items[0].doteStaffId.ToString());
                            doc.ReplaceText("<Designation>", items[0].designation == null ? "" : items[0].designation.ToString());
                            doc.ReplaceText("<DepartmentName>", items[0].departmentName == null ? "" : items[0].departmentName.ToString().Split('-').Last());
                            doc.ReplaceText("<Duty>", items[0].duty == null ? "" : items[0].duty.ToString());
                            doc.ReplaceText("<ValuationCentre>", items[0].valuationCentre == null ? "" : items[0].valuationCentre.ToString());
                        }

                    }
                    else
                    {
                        doc.InsertParagraph();
                        Table t = doc.AddTable(items.Count + 1, 8);
                        t.Alignment = Alignment.center;
                        t.SetWidths(new float[] { 35, 110, 130, 150, 150, 60, 180 });
                        if ((row[5].ToString() == "Theory Exam") || (row[5].ToString() == "Other Exams/Activities"))
                        {
                            t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[1].Paragraphs.First().Append("Name of Faculty").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[2].Paragraphs.First().Append("DOTE Staff ID").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[3].Paragraphs.First().Append("Designation").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[4].Paragraphs.First().Append("Department").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[5].Paragraphs.First().Append("Duty As").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));



                            t.Rows[0].Cells[6].Paragraphs.First().Append("College Code & Name").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[7].Paragraphs.First().Append("College Address").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));



                            for (int k = 0; k < items.Count; k++)
                            {
                                t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                //{ 35, 300, 120, 130, 120 });
                                t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].nameofFaculty == null ? "" : items[k].nameofFaculty.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].doteStaffId == null ? "" : items[k].doteStaffId.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[3].Paragraphs.First().Append(items[k].designation == null ? "" : items[k].designation.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[4].Paragraphs.First().Append(items[k].departmentName == null ? "" : items[k].departmentName == null ? "" : items[k].departmentName.ToString().Split('-').Last()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[5].Paragraphs.First().Append(items[k].dutyAs == null ? "" : items[k].dutyAs.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));



                                t.Rows[k + 1].Cells[6].Paragraphs.First().Append(items[k].collegeCodeandName == null ? "" : items[k].collegeCodeandName.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[7].Paragraphs.First().Append(items[k].collegeAddress == null ? "" : items[k].collegeAddress.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                            }
                        }
                        else if (row[5].ToString() == "Central Valuation/Revaluation")
                        {

                            t.Rows[0].Cells[0].Paragraphs.First().Append("S.No").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[1].Paragraphs.First().Append("Name of Faculty").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[2].Paragraphs.First().Append("DOTE Staff ID").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[3].Paragraphs.First().Append("Designation").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[4].Paragraphs.First().Append("Department").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[5].Paragraphs.First().Append("Duty As").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[6].Paragraphs.First().Append("Subject").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));
                            t.Rows[0].Cells[7].Paragraphs.First().Append("Valuation Centre").Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(82, 82, 82));



                            for (int k = 0; k < items.Count; k++)
                            {
                                t.Rows[k + 1].Cells[0].Paragraphs.First().Append(items[k].sNo.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                //{ 35, 300, 120, 130, 120 });
                                t.Rows[k + 1].Cells[1].Paragraphs.First().Append(items[k].nameofFaculty == null ? "" : items[k].nameofFaculty.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[2].Paragraphs.First().Append(items[k].doteStaffId == null ? "" : items[k].doteStaffId.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[3].Paragraphs.First().Append(items[k].designation == null ? "" : items[k].designation.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[4].Paragraphs.First().Append(items[k].departmentName == null ? "" : items[k].departmentName.ToString().Split('-').Last()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[5].Paragraphs.First().Append(items[k].duty == null ? "" : items[k].duty.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[6].Paragraphs.First().Append(items[k].subject == null ? "" : items[k].subject.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
                                t.Rows[k + 1].Cells[7].Paragraphs.First().Append(items[k].valuationCentre == null ? "" : items[k].valuationCentre.ToString()).Font(new Xceed.Document.NET.Font("Arial")).FontSize(9).Color(Color.FromArgb(59, 56, 56));



                            }
                        }
                        //doc.InsertTable(t);
                        doc.ReplaceTextWithObject("<Table>", t);
                    }
                }
                else
                {
                    var formatcolumn = "<" + col[i] + ">";
                    doc.ReplaceText(formatcolumn, row[i].ToString());
                }
            }
            doc.AddProtection(EditRestrictions.readOnly);
            doc.SaveAs(strfilepath);
            return strfilepath;
        }
        public Task<List<SubjectAttendanceModel>> Getsubjectsformarks(string Department, string Sem,
            string Year, string Section)
        {
            var spName = ConstantSPnames.SP_Getsubjectsformarks;
            return Task.Factory.StartNew(() => _db.Connection.Query<SubjectAttendanceModel>(spName, new
            {
                Department = Department,
                Sem = Sem,
                Year = Year,

                Section = Section
            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public string GetAllMarkReport(string Sem, string Year, string Department, string Section, string subjects, bool AttendanceRequired, string Test)
        {
            try
            {
                var spName = ConstantSPnames.SP_MARKTEMPLATE;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                string[] subjectArr = subjects.Split(',');
                string[] deptArr = Department.Split('-');
                var YearLtr = Year == "1" ? "I" : Year == "2" ? "II" : "III";
                string Semester = YearLtr + "-" + Sem.ToUpper();
                var AttandanceRequired = AttendanceRequired == true ? "Yes" : "No";
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = Sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = Year;
                        objCmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = Convert.ToInt32(deptArr[0]);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = Section;
                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        int StudentCount = ds.Tables[0].Rows.Count;
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            int colCount = 1;
                            int rowCount = 1;
                            int colMaxWidth = subjectArr.Length + 3;
                            int colPart = colMaxWidth / 3;
                            var ws = wb.Worksheets.Add("Mark Template");

                            ws.Cell(rowCount, colCount).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");

                            ws.Cell(++rowCount, colCount).Value = "STUDENT MARK REPORT-" + Test.ToUpper() + " TEST";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");

                            ws.Cell(++rowCount, colCount).Value = "PROGRAMME: " + deptArr[1].ToUpper();
                            ws.Range(rowCount, colCount, rowCount, colPart).Merge().AddToNamed("Titles");

                            ws.Cell(rowCount, colPart + 1).Value = "SEMESTER: " + Semester;
                            ws.Range(rowCount, colPart + 1, rowCount, colPart * 2).Merge().AddToNamed("Titles");

                            colPart = colPart * 2;
                            ws.Cell(rowCount, colPart + 1).Value = "ATTENDANCE REQUIRED: " + AttendanceRequired;
                            ws.Range(rowCount, colPart + 1, rowCount, colMaxWidth).Merge().AddToNamed("Titles");

                            // Column headers
                            ws.Cell(++rowCount, colCount).Value = "SNo";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Titles");

                            ws.Cell(rowCount, ++colCount).Value = "Reg.No";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Titles");

                            ws.Cell(rowCount, ++colCount).Value = "Name of Student";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Titles");

                            // Subjects headers
                            for (int i = 0; i < subjectArr.Length; i++)
                            {
                                ws.Cell(rowCount, i + colCount + 1).Value = subjectArr[i];
                                ws.Column(i + colCount + 1).AdjustToContents().AddToNamed("Titles");
                            }

                            ws.Cell(++rowCount, 1).Value = "Date of Examination";
                            ws.Range(rowCount, 1, rowCount, 3).Merge().AddToNamed("Titles");

                            // Insert Data
                            var rangeWithData = ws.Cell(++rowCount, 1).InsertData(ds.Tables[0]).AddToNamed("Titles");

                            // Styling
                            var titlesStyle = wb.Style;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            titlesStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.InsideBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Font.FontSize = 10;

                            var namedRange = wb.NamedRanges.NamedRange("Titles");
                            if (namedRange != null)
                            {
                                namedRange.Ranges.Style = titlesStyle;
                            }

                            IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(StudentCount + rowCount - 1, colMaxWidth).Address);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            // Delete existing file if it exists
                            if (File.Exists(strfilepath))
                            {
                                File.Delete(strfilepath);
                            }

                            wb.SaveAs(strfilepath);
                        }

                    }
                    return strfilepath;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }
        }
        public string bulkuploadmark(string target, long department, string sem, string year, string section)
        {
            var testtype = "";
            //bool? IsAttandanceRequired=null;
            var AttandanceRequired = "";
            DateTime today = DateTime.Now;

            DataTable dt = new DataTable();
            DataColumn dtColumn;
            dtColumn = new DataColumn();

            dtColumn.DataType = typeof(Int32);
            dtColumn.ColumnName = "Id";
            dtColumn.Caption = "Id";
            dtColumn.AutoIncrement = true;
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "StudentId";
            dtColumn.Caption = "StudentId";
            dtColumn.Unique = true;
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "StudentName";
            dtColumn.Caption = "StudentName";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "Department";
            dtColumn.Caption = "Department";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "Year";
            dtColumn.Caption = "Year";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "Sem";
            dtColumn.Caption = "Sem";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "Section";
            dtColumn.Caption = "Section";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "TestType";
            dtColumn.Caption = "TestType";
            dt.Columns.Add(dtColumn);


            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "Data";
            dtColumn.Caption = "Data";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = Type.GetType("System.String");
            dtColumn.ColumnName = "PreviousMonthAttendance";
            dtColumn.Caption = "PreviousMonthAttendance";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(bool);
            dtColumn.ColumnName = "IsattendanceRequired";
            dtColumn.Caption = "IsattendanceRequired";
            dt.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(bool);
            dtColumn.ColumnName = "ReadytosendEmail";
            dtColumn.Caption = "ReadytosendEmail";
            dt.Columns.Add(dtColumn);


            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(bool);
            dtColumn.ColumnName = "IsParentIntemated";
            dtColumn.Caption = "IsParentIntemated";
            dt.Columns.Add(dtColumn);



            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(DateTime);
            dtColumn.ColumnName = "CreatedDate";
            dtColumn.Caption = "CreatedDate";
            dt.Columns.Add(dtColumn);



            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "createdby";
            dtColumn.Caption = "createdby";
            dt.Columns.Add(dtColumn);
            DataRow dtRow;
            List<string> subject = new List<string>();
            List<string> dateofexam = new List<string>();
            //string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", "C:\\Users\\HP-LD\\Downloads\\Excel Files\\Excel Files\\Student.xslx");
            using (XLWorkbook workBook = new XLWorkbook(target))
            {
                IXLWorksheet workSheet = workBook.Worksheet(1);
                var columncount = workBook.Worksheet(1).LastColumnUsed().ColumnNumber();

                // bool fouthRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    if (row.RangeAddress.FirstAddress.RowNumber == 2)
                    {
                        testtype = row.Cell(1).Value.ToString().Split('-').Last();
                    }
                    if (row.RangeAddress.FirstAddress.RowNumber == 3)
                    {
                        AttandanceRequired = row.LastCellUsed().Value.ToString().Split(' ').Last().ToUpper();
                    }
                    // IsAttandanceRequired = AttandanceRequired == "Yes" ? true : false;
                    if (row.RangeAddress.FirstAddress.RowNumber == 4)
                    {
                        for (int i = 4; i <= 11; i++)
                        {
                            if (row.Cell(i).Value.ToString() != "")
                            {
                                subject.Add(row.Cell(i).Value.ToString().Split('-').Last());
                            }
                        }
                    }
                    if (row.RangeAddress.FirstAddress.RowNumber == 5)
                    {
                        for (int i = 4; i <= 11; i++)
                        {
                            if (row.Cell(i).Value.ToString() != "")
                            {
                                dateofexam.Add(row.Cell(i).Value.ToString().Split(" ").First());
                            }
                        }
                    }
                    //Use the first row to add columns to DataTable.
                    if (row.RangeAddress.FirstAddress.RowNumber > 5)
                    {
                        string data = "";
                        for (int i = 0; i < subject.Count; i++)
                        {
                            data += subject[i] + "-" + row.Cell(i + 4).Value + "    ||  Date : " + dateofexam[i] + ",";
                        }
                        //Add rows to DataTable.
                        dtRow = dt.NewRow();
                        dtRow["Id"] = 1;
                        dtRow["StudentId"] = row.Cell(2).Value.ToString();
                        dtRow["StudentName"] = row.Cell(3).Value.ToString();
                        dtRow["Department"] = department;
                        dtRow["Year"] = year;
                        dtRow["Sem"] = sem;
                        dtRow["Section"] = section;
                        dtRow["TestType"] = testtype.ToLower();
                        dtRow["Data"] = data;
                        dtRow["PreviousMonthAttendance"] = "-";
                        dtRow["IsattendanceRequired"] = AttandanceRequired == "YES" ? true : false;
                        dtRow["ReadytosendEmail"] = 0;
                        dtRow["IsParentIntemated"] = 0;
                        dtRow["CreatedDate"] = today;
                        dtRow["createdby"] = "Admin";
                        dt.Rows.Add(dtRow);
                    }
                }
            }
            // DataTable Exceldt = dt.Tables[0]; //copy data set to datatable
            using (SqlBulkCopy bulkCopy =
                   new SqlBulkCopy(_appSettings.ConnectionInfo.TransactionDatabase))
            {
                var tblName = ConstantSPnames.TBL_STUDENTMARKS;
                bulkCopy.DestinationTableName = tblName;
                try
                {
                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(dt);
                    var sendToDB = new ArrayList();
                    if (AttandanceRequired == "YES")
                    {
                        var spName = ConstantSPnames.SP_UPDATEATTPERCENTAGE;
                        foreach (DataRow row in dt.Rows)
                        {
                            sendToDB.Add(
                                new
                                {
                                    Year = Convert.ToString(row["Year"]),
                                    Sem = Convert.ToString(row["Sem"]),
                                    Section = Convert.ToString(row["Section"]),
                                    StudentId = Convert.ToString(row["StudentId"]),
                                    Department = Convert.ToInt64(row["Department"]),
                                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                                });
                        }
                        Task.Factory.StartNew(() =>
                           _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
                    }
                    return "Uploaded Successfully";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //_logger.LogError(ex.InnerException.ToString());
                    return ex.Message;
                }
            }
        }
        public List<StudentMark> GetStudentMark()
        {
            var spName = ConstantSPnames.SP_GETSTUDENTMARKS;

            using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
            sqlconnection.Open();
            SqlCommand command = new SqlCommand(spName, sqlconnection);
            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader rdr = command.ExecuteReader();
            List<StudentMark> studentmark = new List<StudentMark>();

            while (rdr.Read())
            {

                StudentMark mark = new StudentMark();
                mark.Id = Convert.ToInt32(rdr["Id"]);
                mark.StudentId = rdr["StudentId"].ToString();
                mark.StudentName = rdr["StudentName"].ToString();
                mark.Year = rdr["Year"].ToString();
                mark.Sem = rdr["Sem"].ToString();
                mark.Section = rdr["Section"].ToString();
                mark.Data = rdr["Data"].ToString();
                mark.IsattendanceRequired = Convert.ToBoolean(rdr["IsattendanceRequired"]);
                mark.ReadytosendEmail = Convert.ToBoolean(rdr["ReadytosendEmail"].ToString());
                studentmark.Add(mark);

            }
            rdr.Close();
            command.ExecuteNonQuery();
            return studentmark;
        }

        public Task<List<StudentMark>> InsertStudentMark(StudentMark studentmark)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTSTUDENTMARKS;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentMark>(spName, new
            {
                StudentId = studentmark.StudentId,
                StudentName = studentmark.StudentName,
                Year = studentmark.Year,
                Sem = studentmark.Sem,
                Section = studentmark.Section,
                Data = studentmark.Data,
                IsattendanceRequired = studentmark.IsattendanceRequired,
                ReadytosendEmail = studentmark.ReadytosendEmail,
                createdby = studentmark.createdby,
                //CreatedDate= studentmark.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<FacultySubjectMapping>> GetAllFacultySubMappings(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLFACULTYSUBMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<FacultySubjectMapping>(spName, new
            {
                Id = id
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public string InsertFacultySubMappings(FacultySubjectMapping facultySubjectMapping)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTFACULTYSUBMAP;
            using (SqlConnection sqlconnection =
                    new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                try
                {
                    sqlconnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("FacultyId", SqlDbType.VarChar).Value = facultySubjectMapping.FacultyId;
                    command.Parameters.Add("SubjectId", SqlDbType.BigInt).Value = facultySubjectMapping.SubjectId;
                    command.Parameters.Add("DepartmentId", SqlDbType.BigInt).Value = facultySubjectMapping.DepartmentId;
                    command.Parameters.Add("Sem", SqlDbType.VarChar).Value = facultySubjectMapping.Sem;
                    command.Parameters.Add("Year", SqlDbType.VarChar).Value = facultySubjectMapping.Year;
                    command.Parameters.Add("Section", SqlDbType.VarChar).Value = facultySubjectMapping.Section;
                    command.Parameters.Add("CreatedBy", SqlDbType.VarChar).Value = facultySubjectMapping.CreatedBy;
                    command.Parameters.Add("CreatedDate", SqlDbType.DateTime).Value = facultySubjectMapping.CreatedDate;

                    int result = command.ExecuteNonQuery();
                    sqlconnection.Close();
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

            }
        }


        public string UpdateFacultySubMapping(FacultySubjectMapping facultySubjectMapping)
        {


            var spName = ConstantSPnames.SP_UPDATEFACULTYSUBMAP;
            using (SqlConnection sqlconnection =
                    new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                try
                {
                    sqlconnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Id", SqlDbType.BigInt).Value = facultySubjectMapping.Id;
                    command.Parameters.Add("FacultyId", SqlDbType.VarChar).Value = facultySubjectMapping.FacultyId;
                    command.Parameters.Add("SubjectId", SqlDbType.BigInt).Value = facultySubjectMapping.SubjectId;
                    command.Parameters.Add("DepartmentId", SqlDbType.BigInt).Value = facultySubjectMapping.DepartmentId;
                    command.Parameters.Add("Sem", SqlDbType.VarChar).Value = facultySubjectMapping.Sem;
                    command.Parameters.Add("Year", SqlDbType.VarChar).Value = facultySubjectMapping.Year;
                    command.Parameters.Add("Section", SqlDbType.VarChar).Value = facultySubjectMapping.Section;
                    command.Parameters.Add("ModifiedBy", SqlDbType.VarChar).Value = facultySubjectMapping.ModifiedBy;
                    command.Parameters.Add("ModifiedDate", SqlDbType.DateTime).Value = facultySubjectMapping.ModifiedDate;

                    int result = command.ExecuteNonQuery();
                    sqlconnection.Close();
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

            }
        }


        public Task<List<FacultySubjectMapping>> DeleteFacultySubMapping(int id)
        {
            var spName = ConstantSPnames.SP_DELETEFACULTYSUBMAP;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<FacultySubjectMapping>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<StudentMark>> UpdateReadytosendEmail(bool ReadytosendEmail)
        {



            var spName = ConstantSPnames.SP_UPDATEEMAIL;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentMark>(spName, new
            {
                ReadytosendEmail = ReadytosendEmail
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<Announcement>> GetAnnouncementDetails(int? id, bool isReadToSendData)
        {
            var spName = ConstantSPnames.SP_GETALLANNOUNCEMENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<Announcement>(spName, new
            {
                Id = id,
                IsReadToSendData = isReadToSendData
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        //public Task<List<Announcement>> InsertAnnouncementDetails(Announcement announcement)
        //{
        //    //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
        //    var spName = ConstantSPnames.SP_INSERTANNOUNCEMENTDETAILS;
        //    return Task.Factory.StartNew(() => _db.Connection.Query<Announcement>(spName, new
        //    {
        //        Id = announcement.Id,
        //        AnnouncementDate = announcement.AnnouncementDate,
        //        //Department = announcement.Department,
        //        //Year = announcement.Year,
        //        //Semester = announcement.Semester,
        //        SenderType =announcement.SenderType,
        //        EnglishTranslate = announcement.EnglishTranslate,
        //        TamilTranslate = announcement.TamilTranslate,
        //        IsReadytoSend = announcement.IsReadytoSend,
        //        CreatedBy = announcement.CreatedBy,
        //        CreatedDate = announcement.CreatedDate
        //    }, commandType: CommandType.StoredProcedure).ToList());
        //}

        public async Task<Announcement> InsertAnnouncementDetails(Announcement announcement)
        {
            announcement.IsEmailSend = (announcement.SenderType == "Others" && announcement.othersfilecount == false) ? true : false;
            announcement.PhoneNumber = announcement.PhoneNumber == null ? "" : announcement.PhoneNumber;
            List<string> phoneNumbers = new List<string>();

            var spName = ConstantSPnames.SP_INSERTANNOUNCEMENTDETAILS;


            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {

                try
                {
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlConnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Id", SqlDbType.BigInt).Value = announcement.Id;
                    command.Parameters.Add("AnnouncementDate", SqlDbType.DateTime).Value = announcement.AnnouncementDate;
                    command.Parameters.Add("SenderType", SqlDbType.VarChar).Value = announcement.SenderType;
                    command.Parameters.Add("PhoneNumber", SqlDbType.VarChar).Value = announcement.PhoneNumber;
                    command.Parameters.Add("EnglishTranslate", SqlDbType.NVarChar).Value = announcement.EnglishTranslate;
                    command.Parameters.Add("TamilTranslate", SqlDbType.NVarChar).Value = announcement.TamilTranslate;
                    command.Parameters.Add("IsReadytoSend", SqlDbType.Bit).Value = announcement.IsReadytoSend;
                    command.Parameters.Add("IsEmailSend", SqlDbType.Bit).Value = announcement.IsEmailSend;
                    command.Parameters.Add("CreatedBy", SqlDbType.VarChar).Value = announcement.CreatedBy;
                    command.Parameters.Add("CreatedDate", SqlDbType.DateTime).Value = announcement.CreatedDate;
                    // command.Parameters.Add("outputParameter", SqlDbType.Int).Direction = ParameterDirection.Output;
                    //command.Parameters = emailsend;
                    announcement.Id = Convert.ToInt64(command.ExecuteScalar());

                    sqlConnection.Close();
                    if (announcement.SenderType == "Others" && announcement.othersfilecount == false)
                    {
                        phoneNumbers = announcement.PhoneNumber.Split(',').ToList();
                        //   phoneNumbers = othersphonenumber;


                        var url = _appSettings.SmsSettings.BaseWhatsAppUrl;
                        var Data = announcement.EnglishTranslate + "\n" + announcement.TamilTranslate;

                        var list = phoneNumbers.Select((number, index) => new { number, index }).GroupBy(x => x.index / 500)
                               .Select(g => g.Select(x => x.number).ToList()).ToList();

                        foreach (var item in list)
                        {

                            var client = new RestClient(url);
                            var request = new RestRequest(url, Method.Post);

                            string value = string.Join(",", (item.Select(obj =>

                                "+91" + obj
                            ).ToList()));

                            //value =string.Concat("+91", value);
                            //originalStrings.Select(s => s + suffix).ToList();
                            request.AddHeader("content-type", "application/x-www-form-urlencoded");
                            request.AddParameter("token", "x0p8y2yyxqzz45dg", ParameterType.GetOrPost);
                            request.AddParameter("to", value, ParameterType.GetOrPost);


                            request.AddParameter("body", Data, ParameterType.GetOrPost);


                            request.AddParameter("priority", "5");


                            RestResponse response = client.Execute(request);

                        }
                    }

                    return announcement;
                    //return insertedOrUpdatedId;
                }
                catch (Exception ex)
                {
                    return announcement;
                }
            }
        }
        public Task<List<Announcement>> DeleteAnnouncementDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEANNOUNCEMENTDETAILS;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<Announcement>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }

        //public string DownloadFeedbackQnsReport(long departmentId, long subjectId, long facultyId, string sem, string year, string section)
        //{
        //    try
        //    {
        //        List<QnsModel> qns = new List<QnsModel>();
        //        List<ScoreModel> scoreList = new List<ScoreModel>();
        //        var departmentName = string.Empty;
        //        var subjectName = string.Empty;
        //        var subjectCode = string.Empty;
        //        var facultyName = string.Empty;
        //        var totalCountStudentId = string.Empty;
        //        long ExcellentTotal = 0;
        //        long MediumTotal = 0;
        //        long GoodTotal = 0;
        //        long PoorTotal = 0;
        //        long BadTotal = 0;
        //        long MarkTotal = 0;
        //        double PercentageTotal = 0;
        //        var MaxMark = string.Empty;
        //        var Grade = string.Empty;
        //        var SemReports = string.Empty;
        //        var YearReports = string.Empty;
        //        var feedbackEndDate = string.Empty;
        //        var feedbackStartDate = string.Empty;
        //        var spName = ConstantSPnames.SP_GETFEEDBACKQNSREPORT;
        //        string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
        //        _appSettings.Settings.FileNames.ToString();
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
        //        DataTable ds = new DataTable();
        //        int stdCount = 0;
        //        var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
        //        using (SqlConnection myConnection = new SqlConnection(con))
        //        {
        //            SqlCommand objCmd = new SqlCommand(spName, myConnection);
        //            objCmd.CommandType = CommandType.StoredProcedure;
        //            objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //            objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //            objCmd.Parameters.Add("@DepartmentId", SqlDbType.BigInt).Value = departmentId;
        //            objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section;
        //            objCmd.Parameters.Add("@FacultyId", SqlDbType.BigInt).Value = facultyId;
        //            objCmd.Parameters.Add("@SubjectId", SqlDbType.BigInt).Value = subjectId;
        //            objCmd.CommandTimeout = 100000;
        //            myConnection.Open();
        //            SqlDataReader rdr = objCmd.ExecuteReader();
        //            YearReports = year == "1" ? "I" : year == "2" ? "II" : "III";
        //            if (sem == "Odd")
        //            {
        //                SemReports = (sem == "Odd" && YearReports == "I" ? "1" : sem == "Odd" && YearReports == "II" ? "3" : "5");
        //            }
        //            else if (sem == "Even")
        //            {
        //                SemReports = (sem == "Even" && YearReports == "I" ? "2" : sem == "Even" && YearReports == "II" ? "4" : "6");
        //            }


        //            while (rdr.Read())
        //            {
        //                stdCount++;
        //                int sNo = 0;
        //                totalCountStudentId = Convert.ToString(rdr["TotalCountStudentId"]);

        //                departmentName = Convert.ToString(rdr["DepartmentName"]);
        //                subjectName = Convert.ToString(rdr["SubjectName"]);
        //                subjectCode = Convert.ToString(rdr["SubjectCode"]);
        //                facultyName = Convert.ToString(rdr["FacultyName"]);
        //                feedbackStartDate = Convert.ToString(rdr["FeedbackStartDate"]).Split(" ").First();
        //                feedbackEndDate = Convert.ToString(rdr["FeedbackEndDate"]).Split(" ").First();
        //                qns = JsonConvert.DeserializeObject<List<QnsModel>>(rdr["FeedbackData"].ToString());
        //                foreach (var items in qns)
        //                {
        //                    sNo++;
        //                    ScoreModel scoreCount = new ScoreModel();
        //                    scoreCount.QuestionNo = sNo;
        //                    scoreCount.Question = items.QnsDescription;
        //                    switch (items.Score)
        //                    {
        //                        case "100":
        //                            scoreCount.Excellant++;
        //                            scoreCount.MarksSecured = 5;
        //                            break;
        //                        case "80":
        //                            scoreCount.Good++;
        //                            scoreCount.MarksSecured = 4;
        //                            break;
        //                        case "60":
        //                            scoreCount.Medium++;
        //                            scoreCount.MarksSecured = 3;
        //                            break;
        //                        case "40":
        //                            scoreCount.Poor++;
        //                            scoreCount.MarksSecured = 2;
        //                            break;
        //                        case "20":
        //                            scoreCount.VeryBad++;
        //                            scoreCount.MarksSecured = 1;
        //                            break;
        //                    }
        //                    scoreCount.Feedback = Math.Round(((double)scoreCount.MarksSecured / (stdCount * 5)) * 100, 2);
        //                    if (scoreList.Where(x => x.QuestionNo == (scoreCount.QuestionNo)).Count() == 0)
        //                    {
        //                        scoreList.Add(scoreCount);
        //                    }
        //                    else
        //                    {
        //                        var score = scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault();
        //                        long MarksSecured = 0;
        //                        MarksSecured = scoreCount.MarksSecured;
        //                        if (scoreCount.Excellant == 1)
        //                        {
        //                            scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().Excellant++;
        //                        }
        //                        else if (scoreCount.Good == 1)
        //                        {
        //                            scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().Good++;
        //                        }
        //                        else if (scoreCount.Medium == 1)
        //                        {
        //                            scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().Medium++;
        //                        }
        //                        else if (scoreCount.Poor == 1)
        //                        {
        //                            scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().Poor++;
        //                        }
        //                        else if (scoreCount.VeryBad == 1)
        //                        {
        //                            scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().VeryBad++;
        //                        }
        //                        var Mark = scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().MarksSecured;
        //                        Mark = Mark + MarksSecured;
        //                        var Percentage = Math.Round(((double)Mark / (stdCount * 5)) * 100, 2);
        //                        scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().MarksSecured = Mark;
        //                        scoreList.Where(x => x.QuestionNo == scoreCount.QuestionNo).FirstOrDefault().Feedback = Percentage;
        //                    }
        //                }
        //            }

        //            MaxMark = ((stdCount * 5) * qns.Count()).ToString();
        //            //rdr.NextResult();
        //            //while (rdr.Read())
        //            //{
        //            //    totalCountStudentId = Convert.ToString(rdr["TotalCountStudentId"]);
        //            //}
        //            myConnection.Close();
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
        //            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
        //            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == "QuestionWiseAnalysisForm.docx"));
        //            doc.InsertParagraph();
        //            // doc.SetDefaultFont(new Xceed.Document.NET.Font("Arial"), fontSize: 9, Color.FromArgb(82, 82, 82));
        //            Table t = doc.AddTable(qns.Count + 1, 9);
        //            // t.Design = TableDesign.TableGrid;
        //            // t.Alignment = Alignment.center;

        //            // t.SetWidths(new float[] { 35, 300, 100, 100, 100, 100, 100, 100, 100 });
        //            t.SetWidths(new float[] { 35, 350, 40, 40, 40, 40, 40, 40, 40 });
        //            t.SetTableCellMargin(TableCellMarginType.top, 5d);
        //            t.SetTableCellMargin(TableCellMarginType.bottom, 5d);
        //            // t.SetColumnWidth(0, 50);
        //            t.Rows[0].Cells[0].Paragraphs.First().Append("\n\nQns. No\n").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[1].Paragraphs.First().Append("\n\nQuestions\n").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[2].TextDirection = TextDirection.btLr;
        //            t.Rows[0].Cells[2].Paragraphs.First().Alignment = Alignment.both;
        //            // t.Rows[0].Cells[2].MarginTop = 100;
        //            t.Rows[0].Cells[2].Paragraphs.First().Append("Excellent (5)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[3].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[3].Paragraphs.First().Append("Very Good (4)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[4].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[4].Paragraphs.First().Append("Good (3)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[5].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[5].Paragraphs.First().Append("Satisfactory (2)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[6].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[6].Paragraphs.First().Append("Poor (1)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[7].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[7].Paragraphs.First().Append("Marks Secured").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[8].TextDirection = TextDirection.btLr;

        //            t.Rows[0].Cells[8].Paragraphs.First().Append("Feedback (%)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            // t.AutoFit = AutoFit.Contents;
        //            ExcellentTotal = scoreList.Sum(x => x.Excellant);
        //            MediumTotal = scoreList.Sum(x => x.Medium);
        //            GoodTotal = scoreList.Sum(x => x.Good);
        //            PoorTotal = scoreList.Sum(x => x.Poor);
        //            BadTotal = scoreList.Sum(x => x.VeryBad);
        //            MarkTotal = scoreList.Sum(x => x.MarksSecured);
        //            PercentageTotal = scoreList.Sum(x => x.Feedback);
        //            PercentageTotal = Math.Round((PercentageTotal / 10), 2);

        //            if (PercentageTotal >= 90)
        //            {
        //                Grade = "A+";
        //            }
        //            else if (PercentageTotal >= 80 && PercentageTotal <= 90)
        //            {
        //                Grade = "A";
        //            }
        //            else if (PercentageTotal >= 70 && PercentageTotal <= 80)
        //            {
        //                Grade = "B";
        //            }
        //            else if (PercentageTotal >= 60 && PercentageTotal <= 70)
        //            {
        //                Grade = "C";
        //            }
        //            else if (PercentageTotal <= 60)
        //            {
        //                Grade = "D";
        //            }

        //            for (int k = 0; k < scoreList.Count; k++)
        //            {

        //                t.Rows[k + 1].Cells[0].Paragraphs.First().Append(scoreList[k].QuestionNo.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[1].Paragraphs.First().Append(scoreList[k].Question.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.left;
        //                t.Rows[k + 1].Cells[2].Paragraphs.First().Append(scoreList[k].Excellant.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[3].Paragraphs.First().Append(scoreList[k].Good.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[4].Paragraphs.First().Append(scoreList[k].Medium.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[5].Paragraphs.First().Append(scoreList[k].Poor.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[6].Paragraphs.First().Append(scoreList[k].VeryBad.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[7].Paragraphs.First().Append(scoreList[k].MarksSecured.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[8].Paragraphs.First().Append(scoreList[k].Feedback.ToString("0.00")).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            }
        //            var r = t.InsertRow();
        //            r.MergeCells(0, 1);
        //            r.Cells[0].Paragraphs[0].Append("Total".ToString()).Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.right;


        //            t.Alignment = Alignment.center;

        //            r.Cells[1].Paragraphs[0].Append(ExcellentTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[2].Paragraphs[0].Append(GoodTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[3].Paragraphs[0].Append(MediumTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[4].Paragraphs[0].Append(PoorTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[5].Paragraphs[0].Append(BadTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[6].Paragraphs[0].Append(MarkTotal.ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            r.Cells[7].Paragraphs[0].Append(PercentageTotal.ToString("0.00")).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;

        //            doc.ReplaceTextWithObject("<Table>", t);

        //            doc.ReplaceText("<DepartmentName>", departmentName.ToString());
        //            doc.ReplaceText("<Year>", YearReports.ToString());
        //            doc.ReplaceText("<Sem>", SemReports.ToString());
        //            doc.ReplaceText("<Section>", section.ToString());
        //            doc.ReplaceText("<FacultyName>", facultyName.ToString());
        //            doc.ReplaceText("<SubjectName>", subjectCode.ToString() + "-" + subjectName.ToString());
        //            doc.ReplaceText("<ExcellentTotal>", ExcellentTotal.ToString());
        //            doc.ReplaceText("<MediumTotal>", MediumTotal.ToString());
        //            doc.ReplaceText("<GoodTotal>", GoodTotal.ToString());
        //            doc.ReplaceText("<PoorTotal>", PoorTotal.ToString());
        //            doc.ReplaceText("<BadTotal>", BadTotal.ToString());
        //            doc.ReplaceText("<MarkTotal>", MarkTotal.ToString());
        //            doc.ReplaceText("<PercentageTotal>", PercentageTotal.ToString());
        //            doc.ReplaceText("<Grade>", Grade.ToString());
        //            doc.ReplaceText("<TotalCountstudentId>", totalCountStudentId.ToString());
        //            doc.ReplaceText("<Appeared>", stdCount.ToString());
        //            doc.ReplaceText("<MaxMark>", MaxMark.ToString());
        //            doc.ReplaceText("<StartDate>", feedbackStartDate.ToString());
        //            doc.ReplaceText("<EndDate>", feedbackEndDate.ToString());
        //            var spName1 = ConstantSPnames.SP_INSERTFEEDBACKFACULTYPERCENTAGE;

        //            using (SqlConnection Connection = new SqlConnection(con))
        //            {
        //                Connection.Open();
        //                SqlCommand Cmd = new SqlCommand(spName1, Connection);
        //                Cmd.CommandType = CommandType.StoredProcedure;
        //                Cmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //                Cmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //                Cmd.Parameters.Add("@DepartmentId", SqlDbType.BigInt).Value = departmentId;
        //                Cmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section;
        //                Cmd.Parameters.Add("@FacultyId", SqlDbType.BigInt).Value = facultyId;
        //                Cmd.Parameters.Add("@SubjectId", SqlDbType.BigInt).Value = subjectId;
        //                Cmd.Parameters.Add("@FeedBack", SqlDbType.Decimal).Value = PercentageTotal;
        //                Cmd.Parameters.Add("@Grade", SqlDbType.VarChar).Value = Grade;
        //                Cmd.CommandTimeout = 100000;

        //                Cmd.ExecuteNonQuery();
        //            }
        //            //doc.AddProtection(EditRestrictions.readOnly);
        //            doc.SaveAs(strfilepath);
        //            return strfilepath;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public string DownloadFeedbackFacultyReport(long departmentId, string sem, string year, string section = null)
        //{
        //    try
        //    {
        //        List<FacultyFeedbackModel> facultyName = new List<FacultyFeedbackModel>();
        //        List<CourseModel> courseList = new List<CourseModel>();
        //        var spName = ConstantSPnames.SP_GETFEEDBACKFACULTYPERCENTAGEREPORT;
        //        string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
        //        DataTable ds = new DataTable();

        //        long subjectId = 0;
        //        var subjectName = string.Empty;
        //        long facultyId = 0;
        //        double PercentageTotal = 0;

        //        var departmentName = string.Empty;
        //        var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
        //        using (SqlConnection myConnection = new SqlConnection(con))
        //        {
        //            SqlCommand objCmd = new SqlCommand(spName, myConnection);
        //            objCmd.CommandType = CommandType.StoredProcedure;
        //            objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //            objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //            objCmd.Parameters.Add("@DepartmentId", SqlDbType.BigInt).Value = departmentId;
        //            if (section != null) { objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section; }
        //            objCmd.CommandTimeout = 100000;
        //            myConnection.Open();
        //            using (var da = new SqlDataAdapter(objCmd))
        //            {
        //                da.Fill(ds);
        //            }
        //            var row = ds.Rows;
        //            for (int i = 0; i < row.Count; i++)
        //            {
        //                subjectId = (long)row[i].ItemArray[0];
        //                subjectName = row[i].ItemArray[1].ToString();
        //                facultyName = JsonConvert.DeserializeObject<List<FacultyFeedbackModel>>(row[i].ItemArray[2].ToString());
        //                foreach (var items in facultyName)
        //                {
        //                    facultyId = items.Id;
        //                    DownloadFeedbackQnsReport(departmentId, subjectId, facultyId, sem, year, section);
        //                }
        //                myConnection.Close();
        //            }
        //        }
        //        var spNametable = ConstantSPnames.SP_GETFEEDBACKFACULTYPERCENTAGE;
        //        DataTable dt = new DataTable();
        //        using (SqlConnection tableConnection = new SqlConnection(con))
        //        {
        //            SqlCommand tableCmd = new SqlCommand(spNametable, tableConnection);
        //            tableCmd.CommandType = CommandType.StoredProcedure;
        //            tableCmd.Parameters.Add("@DepartmentId", SqlDbType.BigInt).Value = departmentId;
        //            tableCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //            tableCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //            if (section != null) { tableCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section; }
        //            tableConnection.Open();
        //            tableCmd.CommandTimeout = 100000;
        //            tableCmd.ExecuteNonQuery();
        //            using (var dttable = new SqlDataAdapter(tableCmd))
        //            {
        //                dttable.Fill(dt);
        //            }
        //            var row = dt.Rows;
        //            var column = dt.Columns;
        //            departmentName = row[0].ItemArray[5].ToString();
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
        //            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
        //            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == "TeachingWiseForm.docx"));
        //            doc.InsertParagraph();
        //            Table t = doc.AddTable(row.Count + 1, 5);
        //            t.SetTableCellMargin(TableCellMarginType.top, 10d);
        //            t.SetTableCellMargin(TableCellMarginType.bottom, 10d);
        //            t.SetWidths(new float[] { 30, 350, 180, 140, 100 });

        //            t.Alignment = Alignment.center;
        //            t.Rows[0].Cells[0].Paragraphs.First().Append("S.No.").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;

        //            t.Rows[0].Cells[1].Paragraphs.First().Append("Course Name").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[2].Paragraphs.First().Append("Faculty Name").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[3].Paragraphs.First().Append("Feedback-(%)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[4].Paragraphs.First().Append("Letter Grade").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            for (int k = 0; k < row.Count; k++)
        //            {
        //                t.Rows[k + 1].Cells[0].Paragraphs.First().Append(row[k].ItemArray[0].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[1].Paragraphs.First().Append(row[k].ItemArray[1].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
        //                t.Rows[k + 1].Cells[2].Paragraphs.First().Append(row[k].ItemArray[2].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
        //                t.Rows[k + 1].Cells[3].Paragraphs.First().Append(row[k].ItemArray[3].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[4].Paragraphs.First().Append(row[k].ItemArray[4].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            }

        //            doc.ReplaceTextWithObject("<Table>", t);
        //            year = year == "1" ? "I" : year == "2" ? "II" : "III";
        //            if (sem == "Odd")
        //            {
        //                sem = (sem == "Odd" && year == "I" ? "1" : sem == "Odd" && year == "II" ? "3" : "5");
        //            }
        //            else if (sem == "Even")
        //            {
        //                sem = (sem == "Even" && year == "I" ? "2" : sem == "Even" && year == "II" ? "4" : "6");
        //            }
        //            doc.ReplaceText("<DepartmentName>", departmentName.ToString());
        //            doc.ReplaceText("<Year>", year.ToString());
        //            doc.ReplaceText("<Sem>", sem.ToString());
        //            section = (section != null) ? section : "";
        //            doc.ReplaceText("<Section>", section.ToString());


        //           // doc.AddProtection(EditRestrictions.readOnly);
        //            doc.SaveAs(strfilepath);
        //            return strfilepath;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public string DeleteMark(List<StudentMark> mark)
        {
            var spName = ConstantSPnames.SP_DELETEMARK;
            var sendToDB = new ArrayList();
            try
            {
                foreach (var item in mark)
                {
                    sendToDB.Add(
                        new
                        {
                            Id = item.Id
                        });
                }
                var delte = _db.Connection.Execute(spName, sendToDB.ToArray(),
                    commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public Task<List<FacultySubjectMapping>> getFacultySubjectforfeedback(int Department, string Sem, string Year, string Section, int Subject)
        {
            var spName = ConstantSPnames.getFacultySubjectforfeedback;
            return Task.Factory.StartNew(() => _db.Connection.Query<FacultySubjectMapping>(spName, new
            {
                Department = Department,
                Sem = Sem,
                Year = Year,
                Subject = Subject,
                Section = Section,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public string DownloadSemWiseFeedbackReport(string departmentId, string sem, string year, string section, bool isSubmitted)
        {
            try
            {
                var spName = ConstantSPnames.SP_GETSECWISEFEEDBACKREPORTS;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                string[] deptArr = departmentId.Split('-');
                var YearLtr = year == "1" ? "I" : year == "2" ? "II" : "III";
                string Semester = YearLtr + "-" + sem.ToUpper();

                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
                        objCmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = Convert.ToInt32(deptArr[0]);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section;
                        objCmd.Parameters.Add("@IsSubmitted", SqlDbType.Bit).Value = isSubmitted;
                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        int StudentCount = ds.Tables[0].Rows.Count;
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            int colCount = 1;
                            int rowCount = 1;

                            int colMaxWidth = ds.Tables[0].Columns.Count; ;
                            // int colPart = colMaxWidth / 3;
                            var ws = wb.Worksheets.Add("Section Wise Feedback Reports");
                            ws.Cell(rowCount, colCount).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "FEEDBACK GIVEN STUDENT LIST";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "PROGRAMME NAME: " + deptArr[1].ToUpper();
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "SEMESTER: " + Semester;
                            ws.Range(rowCount, colCount, rowCount, 2).Merge().AddToNamed("Titles");
                            // colPart = colPart * 2;
                            ws.Cell(rowCount, 3).Value = "SECTION: " + section;
                            ws.Range(rowCount, 3, rowCount, 4).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "S.No";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Reg.No";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Name of Student";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Feedback Given Student";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            var rangeWithData = ws.Cell(++rowCount, 1).InsertData(ds.Tables[0]).AddToNamed("Titles");
                        
                            var titlesStyle = wb.Style;
                            //titlesStyle.Font.Bold = true;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Alignment.WrapText = true;
                            titlesStyle.Font.SetFontSize(10);
                           // wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                            var namedRange = wb.NamedRanges.NamedRange("Titles");

                            if (namedRange != null)
                            {
                                namedRange.Ranges.Style = titlesStyle;
                            }
                           

                            IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(StudentCount + rowCount - 1, colMaxWidth).Address);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;



                            if (File.Exists(strfilepath))
                            {
                                File.Delete(strfilepath);
                            }
                            //lblerror.Text = "three";
                            wb.SaveAs(strfilepath);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                        }
                    }
                    return strfilepath;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }
        }



        public string UpdateStdconfigFeedback(long? Department, bool IsFeebackSend)
        {
            var spName = ConstantSPnames.SP_UPDATEFEEDBACK;
            try
            {
                using (SqlConnection sqlconnection =
                       new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    sqlconnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlconnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Department", SqlDbType.BigInt).Value = Department;
                    command.Parameters.Add("IsFeedbackSend", SqlDbType.Bit).Value = IsFeebackSend;
                    command.ExecuteNonQuery();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
        }
        public Task<List<StudentSemDateModel>> GetAllStudentConfiguration(int? id)
        {
            var spName = ConstantSPnames.SP_GETSTDCONGIG;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<StudentSemDateModel>(spName, commandType: CommandType.StoredProcedure)
                    .ToList());
        }
        public Task<List<StudentSemDateModel>> updateStudentSemDateDetails(StudentSemDateModel studentSemDate)
        {
            var spName = ConstantSPnames.SP_UPDATESTUDENTSEMDATE;
            return Task.Factory.StartNew(() =>
            _db.Connection.Query<StudentSemDateModel>(spName, new
            {
                Sem = studentSemDate.Sem,
                FirstYearStartDate = studentSemDate.FirstYearStartDate,
                FirstYearEndDate = studentSemDate.FirstYearEndDate,
                SecondYearStartDate = studentSemDate.SecondYearStartDate,
                SecondYearEndDate = studentSemDate.SecondYearEndDate,
                ThirdYearStartDate = studentSemDate.ThirdYearStartDate,
                ThirdYearEndDate = studentSemDate.ThirdYearEndDate,
                FeedbackStartDate = studentSemDate.FeedbackStartDate,
                FeedbackEndDate = studentSemDate.FeedbackEndDate,
                ModifiedBy = studentSemDate.ModifiedBy,
                ModifiedDate = studentSemDate.ModifiedDate
            }, commandType: CommandType.StoredProcedure)
                .ToList());
        }
        public string GenerateMarkReport(string departmentId, string sem, string year, string section, string test)
        {
            try
            {
                var spName = ConstantSPnames.SP_GETMARKSMSREPORTS;
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                     _appSettings.Settings.FileName.ToString();
                DataTable dtTable = new DataTable();
                string[] deptArr = departmentId.Split('-');
                var YearLtr = year == "1" ? "I" : year == "2" ? "II" : "III";
                string Semester = YearLtr + "-" + sem.ToUpper();

                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                using (SqlConnection myConnection = new SqlConnection(con))
                {
                    SqlCommand objCmd = new SqlCommand(spName, myConnection);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    using (var da = new SqlDataAdapter(objCmd))
                    {
                        DataSet ds = new DataSet();
                        objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
                        objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
                        objCmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = Convert.ToInt32(deptArr[0]);
                        objCmd.Parameters.Add("@Section", SqlDbType.VarChar).Value = section;
                        objCmd.Parameters.Add("@Test", SqlDbType.VarChar).Value = test.ToLower() + " test";


                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        int StudentCount = ds.Tables[0].Rows.Count;


                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            int colCount = 1;
                            int rowCount = 1;
                            int colMaxWidth = 5;
                            int colPart = colMaxWidth / 2;
                            var ws = wb.Worksheets.Add("Mark Reports");
                            ws.Cell(rowCount, colCount).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "STUDENT- MARKS SENT LIST- " + test.ToUpper() + " TEST";
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "PROGRAMME: " + deptArr[1].ToUpper();
                            ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "SEMESTER: " + Semester;
                            ws.Range(rowCount, colCount, rowCount, colPart * 2).Merge().AddToNamed("Titles");
                            colPart = colPart * 2;
                            ws.Cell(rowCount, colPart + 1).Value = "SECTION: " + section;
                            ws.Range(rowCount, colPart + 1, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                            ws.Cell(++rowCount, colCount).Value = "SNo";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Register.No";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Name of Student";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Exam Marks With Date";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");
                            ws.Cell(rowCount, ++colCount).Value = "Pre.Attendance-%";
                            ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");

                            var rangeWithData = ws.Cell(++rowCount, 1).InsertData(ds.Tables[0]).AddToNamed("Titles");


                            var titlesStyle = wb.Style;
                            //titlesStyle.Font.Bold = true;
                            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                            titlesStyle.Alignment.WrapText = true;
                            titlesStyle.Font.SetFontSize(10);
                            // wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                            var namedRange = wb.NamedRanges.NamedRange("Titles");

                            if (namedRange != null)
                            {
                                namedRange.Ranges.Style = titlesStyle;
                            }
                            else
                            {
                                Console.WriteLine("Named range 'Titles' not found.");
                            }

                            IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(StudentCount + rowCount - 1, colMaxWidth).Address);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;



                            if (File.Exists(strfilepath))
                            {
                                File.Delete(strfilepath);
                            }
                            //lblerror.Text = "three";
                            wb.SaveAs(strfilepath);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                        }
                    }
                    return strfilepath;
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }
        }
        public string SendStudentFeeedback(string StudentId)
        {
            try
            {
                var StudentID = JsonConvert.DeserializeObject<List<Student>>(StudentId.ToString());
                string StudentIdList = string.Join(",", StudentID.Select(x => x.StudentID));
                //RsaEncryption rsa = new RsaEncryption();
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                List<FeedbackListModel> feedbackLists = new List<FeedbackListModel>();
                var spName = "[sp_GetFeedbackStudentwise]";
                using SqlConnection sqlconnection = new SqlConnection(con);
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@StudentId", SqlDbType.VarChar).Value = StudentIdList;
                SqlDataReader result = command.ExecuteReader();

                while (result.Read())
                {
                    FeedbackListModel feedbackList = new FeedbackListModel()
                    {
                        StudentId = Convert.ToString(result["StudentId"]),
                        StudentName = Convert.ToString(result["StudentName"]),
                        Father_MobileNumber = Convert.ToString(result["StudentMobileNumber_1"]),
                        FeedbackEndDate = Convert.ToString(result["FeedbackEndDate"])
                    };
                    feedbackLists.Add(feedbackList);
                }
                sqlconnection.Close();

                foreach (var item in feedbackLists)
                {
                    var Params = "";
                    if (!string.IsNullOrEmpty(item.StudentId))
                    {
                        Params = (item.StudentId);
                    }

                    if (item.Father_MobileNumber.Length == 10)
                    {
                        item.Father_MobileNumber = $"+91{item.Father_MobileNumber}";
                    }
                    var feedbackUrl = $"{_appSettings.SmsSettings.FeedbackSendUrl}{Params}";
                   
                    var FormattedData = $"Dear {item.StudentName}, your feedback survey is open now and available only until {item.FeedbackEndDate.Split(' ').First()}. Please visit the following link to access.\n-TPT College \n{feedbackUrl}";

                    // var FormattedData = "Dear " + item.StudentName + ", your feedback survey is open now and available only until " + item.FeedbackEndDate.Split(' ').First() + " Please click the below link.. TPT College \n: {feedbackUrl}";
                    //var Data = HttpUtility.UrlEncode(FormattedData);

                    var url = _appSettings.SmsSettings.BaseWhatsAppUrl;
                    var client = new RestClient(url);

                    var request = new RestRequest(url, Method.Post);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddParameter("token", "x0p8y2yyxqzz45dg");
                    request.AddParameter("to", item.Father_MobileNumber);
                    request.AddParameter("body", FormattedData);
                    RestResponse response = client.Execute(request);
                    if (response.StatusCode.ToString() != "OK")
                    {
                        return (response.Content.ToString());
                    }
                    var output = response.Content;
                    // //https://localhost:44374/Feedback/createForm?studentId=1&sem=Odd&year=1&section=A&DepartmentId=32
                    // var Params = "";
                    // if (!string.IsNullOrEmpty(item.StudentId))
                    // {
                    //     Params = (item.StudentId);
                    // }
                    //// var url = "http://promo.smso2.com/api/sendhttp.php?authkey=38385043534c4d31303568&mobiles=" + item.Father_MobileNumber + "&message=" + Data + "&sender=TPTSLM&route=2&country=91&DLT_TE_ID=" + TempId;
                    // // var feedbackUrl = "https://localhost:44374/Feedback/createForm?data=" + Params;
                    // var FbLastDate = item.FeedbackEndDate.Split(' ').First() + ",";
                    // var FormattedData = "Dear " + item.StudentName + ", Your feedback survey is open now and available only until " + FbLastDate + " Please click the below link http://103.53.52.215:81/createForm?data=" + Params + " TPT College";
                    // var Data = HttpUtility.UrlEncode(FormattedData);
                    // var url = _appSettings.SmsSettings.BaseSmsUrl + item.Father_MobileNumber + "&message=" + Data + "&sender=TPTSLM&route=2&country=91&DLT_TE_ID=" + _appSettings.SmsSettings.token;
                    // var client = new RestClient(url);
                    // var request = new RestRequest(url, Method.Post);
                    // RestResponse response = client.Execute(request);
                    // if (response.StatusCode.ToString() != "OK")
                    // {
                    //     return (response.Content.ToString());
                    // }
                    //var output = response.Content;
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public Task<List<StudentDropdown>> GetMappedStudentByName(string StudentName, int DepartmentId, string Sem, string Year)
        {
            var spName = ConstantSPnames.SP_GETMAPPEDSTUDENTBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentDropdown>(spName, new
            {
                StudentName = StudentName,
                DepartmentId = DepartmentId,
                Sem = Sem,
                Year = Year

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        //public string DownloadYearWiseSecFeedbackReport(long subjectId, string sem, string year)
        //{
        //    try
        //    {
        //        List<FacultyFeedbackModel> facultyName = new List<FacultyFeedbackModel>();
        //        List<CourseModel> courseList = new List<CourseModel>();
        //        var spName = ConstantSPnames.SP_GETFEEDBACKFACULTYPERCENTAGEREPORT;
        //        string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" + _appSettings.Settings.FileNames.ToString();
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Logo", "TptLogo.png");
        //        DataTable ds = new DataTable();

        //        var section = string.Empty;
        //        var subjectName = string.Empty;
        //        long facultyId = 0;
        //        var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
        //        using (SqlConnection myConnection = new SqlConnection(con))
        //        {
        //            SqlCommand objCmd = new SqlCommand(spName, myConnection);
        //            objCmd.CommandType = CommandType.StoredProcedure;
        //            objCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //            objCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //            objCmd.Parameters.Add("@subjectId", SqlDbType.BigInt).Value = subjectId;

        //            objCmd.CommandTimeout = 100000;
        //            myConnection.Open();
        //            using (var da = new SqlDataAdapter(objCmd))
        //            {
        //                da.Fill(ds);
        //            }
        //            var row = ds.Rows;
        //            for (int i = 0; i < row.Count; i++)
        //            {
        //                subjectId = (long)row[i].ItemArray[0];
        //                subjectName = row[i].ItemArray[1].ToString();
        //                section = row[i].ItemArray[3].ToString();
        //                int departmentId = Convert.ToInt32(row[i].ItemArray[4]);
        //                facultyName = JsonConvert.DeserializeObject<List<FacultyFeedbackModel>>(row[i].ItemArray[2].ToString());
        //                foreach (var items in facultyName)
        //                {
        //                    facultyId = items.Id;
        //                    DownloadFeedbackQnsReport(departmentId, subjectId, facultyId, sem, year, section);
        //                }
        //                myConnection.Close();
        //            }
        //        }
        //        var spNametable = ConstantSPnames.SP_GETFEEDBACKEPORTSECTIONWISEWITHSUBREPORT;
        //        DataTable dt = new DataTable();
        //        using (SqlConnection tableConnection = new SqlConnection(con))
        //        {
        //            SqlCommand tableCmd = new SqlCommand(spNametable, tableConnection);
        //            tableCmd.CommandType = CommandType.StoredProcedure;
        //            tableCmd.Parameters.Add("@Sem", SqlDbType.VarChar).Value = sem;
        //            tableCmd.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
        //            tableCmd.Parameters.Add("@SubjectId", SqlDbType.BigInt).Value = subjectId;
        //            tableConnection.Open();
        //            tableCmd.CommandTimeout = 100000;
        //            tableCmd.ExecuteNonQuery();
        //            using (var dttable = new SqlDataAdapter(tableCmd))
        //            {
        //                dttable.Fill(dt);
        //            }
        //            var row = dt.Rows;
        //            var column = dt.Columns;
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "FormTemplate");
        //            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
        //            var doc = DocX.Load(files.Find(x => Path.GetFileName(x) == "BatchWiseForm.docx"));
        //            doc.InsertParagraph();
        //            Table t = doc.AddTable(row.Count + 1, 6);
        //            t.SetTableCellMargin(TableCellMarginType.top, 10d);
        //            t.SetTableCellMargin(TableCellMarginType.bottom, 10d);
        //            t.SetWidths(new float[] { 30, 300, 100, 180, 100, 80 });

        //            t.Alignment = Alignment.center;
        //            t.Rows[0].Cells[0].Paragraphs.First().Append("S.No.").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[1].Paragraphs.First().Append("Department Name").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[2].Paragraphs.First().Append("Batch Name").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[3].Paragraphs.First().Append("Faculty Name").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[4].Paragraphs.First().Append("Feedback-(%)").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            t.Rows[0].Cells[5].Paragraphs.First().Append("Letter Grade").Bold(true).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(82, 82, 82)).Alignment = Alignment.center;
        //            for (int k = 0; k < row.Count; k++)
        //            {
        //                t.Rows[k + 1].Cells[0].Paragraphs.First().Append(row[k].ItemArray[0].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[1].Paragraphs.First().Append(row[k].ItemArray[5].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56));
        //                t.Rows[k + 1].Cells[2].Paragraphs.First().Append(row[k].ItemArray[2].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[3].Paragraphs.First().Append(row[k].ItemArray[1].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[4].Paragraphs.First().Append(row[k].ItemArray[3].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //                t.Rows[k + 1].Cells[5].Paragraphs.First().Append(row[k].ItemArray[4].ToString()).Font(new Xceed.Document.NET.Font("Segoe UI")).FontSize(9).Color(Color.FromArgb(59, 56, 56)).Alignment = Alignment.center;
        //            }

        //            doc.ReplaceTextWithObject("<Table>", t);
        //            year = year == "1" ? "I" : year == "2" ? "II" : "III";
        //            if (sem == "Odd")
        //            {
        //                sem = (sem == "Odd" && year == "I" ? "1" : sem == "Odd" && year == "II" ? "3" : "5");
        //            }
        //            else if (sem == "Even")
        //            {
        //                sem = (sem == "Even" && year == "I" ? "2" : sem == "Even" && year == "II" ? "4" : "6");
        //            }
        //            doc.ReplaceText("<CourseName>", subjectName.ToString());
        //            doc.ReplaceText("<Year>", year.ToString());
        //            doc.ReplaceText("<Sem>", sem.ToString());


        //            doc.AddProtection(EditRestrictions.readOnly);
        //            doc.SaveAs(strfilepath);
        //            return strfilepath;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public string GenerateStdAssReports(int departmentId)
        {
            try
            {
                var spName = ConstantSPnames.SP_GETSTUDENTASSREPORTDATA;
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy_MM_dd-HH_mm_ss");
                string strfilepath = _appSettings.Settings.DownloadPath.ToString() + "\\" +
                                    formattedDate;
                Directory.CreateDirectory(strfilepath);
                DataTable dtTable = new DataTable();
                //string[] deptArr = departmentId.Split('-');
                string Semester = string.Empty;
                string Section = string.Empty;
                List<StdAssessmenrReportModel> stdAssessmenrReports = new List<StdAssessmenrReportModel>();
                List<string[]> headers = new List<string[]> { new string[] { "SNo", "Time Stamp", "Register No", "Student Name", "Email", "Department", "Year", "Sem", "Section", "Score" } };
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();

                using SqlConnection sqlconnection = new SqlConnection(con);
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar).Value = departmentId;
                SqlDataReader result = command.ExecuteReader();

                while (result.Read())
                {

                    stdAssessmenrReports.Add(new StdAssessmenrReportModel
                    {
                        SNo = Convert.ToInt32(result["SNo"]),
                        TimeStamp = Convert.ToString(result["TimeStamp"]).Split(' ')[0],
                        StudentName = Convert.ToString(result["StudentName"]),
                        RegisterNo = Convert.ToString(result["RegisterNo"]),
                        Email = Convert.ToString(result["Email"]),
                        Department = Convert.ToString(result["Department"]),
                        Year = Convert.ToString(result["Year"]),
                        Sem = Convert.ToString(result["Sem"]),
                        Section = Convert.ToString(result["Section"]),
                        Score = Convert.ToString(result["Score"])
                    });
                }
                sqlconnection.Close();


                var DeptArr = stdAssessmenrReports.GroupBy(x => new { x.Department, x.Year, x.Section }).ToList();
                for (int i = 0; i < DeptArr.Count; i++)
                {
                    var excelName = strfilepath + "\\" + DeptArr[i].Key.Department + " " + DeptArr[i].Key.Year + " " + DeptArr[i].Key.Section + ".xlsx";
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        int colCount = 1;
                        int rowCount = 1;
                        int colMaxWidth = headers[0].Length;

                        var ws = wb.Worksheets.Add();
                        ws.Cell(rowCount, colCount).Value = "THIAGARAJAR POLYTECHNIC COLLEGE, SALEM - 636005";
                        ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");
                        ws.Cell(++rowCount, colCount).Value = "STUDENT-ASSESSMENT REPORT ";
                        ws.Range(rowCount, colCount, rowCount, colMaxWidth).Merge().AddToNamed("Titles");

                        ws.Cell(++rowCount, 1).InsertData(headers).AddToNamed("Titles");
                        ws.Column(colCount).AdjustToContents().AddToNamed("Tittles");

                        var rangeWithData = ws.Cell(++rowCount, 1).InsertData(DeptArr[i]).AddToNamed("Titles");


                        var titlesStyle = wb.Style;
                        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        titlesStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
                        titlesStyle.Alignment.WrapText = true;
                        titlesStyle.Font.SetFontSize(10);
                        // wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;
                        var namedRange = wb.NamedRanges.NamedRange("Titles");

                        if (namedRange != null)
                        {
                            namedRange.Ranges.Style = titlesStyle;
                        }
                        else
                        {
                            Console.WriteLine("Named range 'Titles' not found.");
                        }
                        var stdLength = stdAssessmenrReports.Count(x => x.Department == DeptArr[i].Key.Department && x.Year == DeptArr[i].Key.Year && x.Section == DeptArr[i].Key.Section);
                        IXLRange range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(stdLength + rowCount - 1, colMaxWidth).Address);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;



                        if (File.Exists(excelName))
                        {
                            File.Delete(excelName);
                        }
                        //lblerror.Text = "three";
                        wb.SaveAs(excelName);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
                return strfilepath;
            }


            catch (Exception ex)
            {
                //_logger.LogError(ex.InnerException.ToString());
                return ex.Message;
            }
        }
        public Task<List<MemberDetails>> GetMembersDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETMEMBERS;
            return Task.Factory.StartNew(() => _db.Connection.Query<MemberDetails>(spName, new
            {
                Id = id
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<MemberDetails>> InsertMembersDetails(MemberDetails memberDetails)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTMEMBERS;
            return Task.Factory.StartNew(() => _db.Connection.Query<MemberDetails>(spName, new
            {

                Name = memberDetails.Name,
                PhoneNumber = memberDetails.PhoneNumber,
                MemberType = memberDetails.MemberType,
                Address = memberDetails.Address,
                Email = memberDetails.Email,
                Gender = memberDetails.Gender,
                CreatedBy = memberDetails.CreatedBy,
                CreatedDate = memberDetails.CreatedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public Task<List<MemberDetails>> UpdateMembersDetails(MemberDetails memberDetails)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_UPDATEMEMBERS;
            return Task.Factory.StartNew(() => _db.Connection.Query<MemberDetails>(spName, new
            {
                Id = memberDetails.Id,
                Name = memberDetails.Name,
                PhoneNumber = memberDetails.PhoneNumber,
                MemberType = memberDetails.MemberType,
                Address = memberDetails.Address,
                Email = memberDetails.Email,
                Gender = memberDetails.Gender,
                ModifiedBy = memberDetails.ModifiedBy,
                ModifiedDate = memberDetails.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<MemberDetails>> DeleteMembersDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEMEMBERS;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<MemberDetails>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        
        public string SendWhatsAppMsg(long id, string formName, string receiverRoleName, string message, string senderName)
        {
            try
            {
                //var StudentID = JsonConvert.DeserializeObject<List<Student>>(StudentId.ToString());
                //string StudentIdList = string.Join(",", StudentID.Select(x => x.StudentID));
                //RsaEncryption rsa = new RsaEncryption();
                var con = _appSettings.ConnectionInfo.TransactionDatabase.ToString();
                List<FormFacultyNoModel> facultyNumList = new List<FormFacultyNoModel>();
                var spName = formName == "INDENT" ? ConstantSPnames.SP_GETINDENTFACULTYMBLNUMBER : ConstantSPnames.SP_GETFDPFACULTYMBLNUMBER;
                using SqlConnection sqlconnection = new SqlConnection(con);
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = id;
                command.Parameters.Add("@ReceiverRoleName", SqlDbType.VarChar).Value = receiverRoleName;
                SqlDataReader result = command.ExecuteReader();
                if (formName == "INDENT")
                {
                    while (result.Read())
                    {
                        FormFacultyNoModel facultyNum = new FormFacultyNoModel()
                        {
                            WhatsAppNum = Convert.ToString(result["WhatsAppNum"]),
                            FormDate = Convert.ToString(result["FormDate"]),
                            Message = message,
                            senderName = senderName,
                            ProgrammeName = Convert.ToString(result["ProgrammeName"]),
                            NameOfTheLaboratory = Convert.ToString(result["NameOfTheLaboratory"]),
                            Purpose = Convert.ToString(result["Purpose"]),


                        };
                        facultyNumList.Add(facultyNum);
                    }
                }
                else
                {
                    while (result.Read())
                    {
                        FormFacultyNoModel facultyNum = new FormFacultyNoModel()
                        {
                            WhatsAppNum = Convert.ToString(result["WhatsAppNum"]),
                            FormDate = Convert.ToString(result["FormDate"]),
                            Message = message,
                            senderName = senderName,
                            ProgrammeName = Convert.ToString(result["ProgrammeName"]),
                            TitleOfTheProgramme = Convert.ToString(result["TitleOfTheProgramme"]),
                            FacultyName = Convert.ToString(result["FacultyName"]),

                        };
                        facultyNumList.Add(facultyNum);
                    }
                }
                sqlconnection.Close();


                var url = _appSettings.SmsSettings.BaseWhatsAppUrl;
                var client = new RestClient(url);
                var numList = string.Join(",", (facultyNumList.Select(obj =>
                {
                    obj.WhatsAppNum = string.Concat("+91", obj.WhatsAppNum);
                    return obj.WhatsAppNum;
                })));
                var Data = formName == "INDENT" ? "From " + senderName + "\n" + facultyNumList[0].Message + "\n\nDetails For Reference\nIndent Request Date: " + facultyNumList[0].FormDate.Split(" ")[0]
                 + "\nName of the Programme: " + facultyNumList[0].ProgrammeName + "\nName of the Laboratory: " + facultyNumList[0].NameOfTheLaboratory + "\nPurpose: " + facultyNumList[0].Purpose : "From " + senderName + "\n" + facultyNumList[0].Message + "\n\nDetails For Reference\nFDP Request Date: " + facultyNumList[0].FormDate.Split(" ")[0]
                 + "\nProgramme Name: " + facultyNumList[0].ProgrammeName + "\nFaculty Name: " + facultyNumList[0].FacultyName + "\nTitle of the Programme: " + facultyNumList[0].TitleOfTheProgramme;
                var request = new RestRequest(url, Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", _appSettings.SmsSettings.token);
                request.AddParameter("to", numList);
                request.AddParameter("body", Data);
                request.AddParameter("priority", "5");
                //request.AddParameter("link", feedbackUrl);
                RestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    return "Success";
                }
                //var output = response.Content;
                //var output = response.Content;
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UploadBulkAnnouncementFile(string target, string target1, string bulkfilenames, int id, string filesList, string EnglishTranslate, string TamilTranslate, string OthersPhoneNumber, string SenderType)
        {
            var emailsend = (SenderType == "Others" || SenderType == "Members") ? 1 : 0;
            var spName = ConstantSPnames.SP_UPDATEANNOUNCEMENTFILE;


            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                try
                {
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand(spName, sqlConnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Id", SqlDbType.BigInt).Value = id;
                    command.Parameters.Add("filepath", SqlDbType.VarChar).Value = target;
                    command.Parameters.Add("files", SqlDbType.VarChar).Value = filesList;
                    command.Parameters.Add("bulkfilepath", SqlDbType.VarChar).Value = target1;
                    command.Parameters.Add("bulkfiles", SqlDbType.VarChar).Value = bulkfilenames;
                    command.Parameters.Add("IsEmailSend", SqlDbType.Bit).Value = emailsend;
                    //command.Parameters = emailsend;
                    var result = Convert.ToInt64(command.ExecuteScalar());
                    sqlConnection.Close();
                    //if (result == 1)
                    //{
                    //List<string> phoneNumbers = new List<string>();
                    if (SenderType == "Members" || SenderType == "Others")
                    {


                        List<string> phoneNumbers = new List<string>();

                        if (SenderType == "Members")
                        {
                            string[] path = Directory.GetFiles(target1);
                            using (XLWorkbook workBook = new XLWorkbook(path[0]))
                            {
                                IXLWorksheet workSheet = workBook.Worksheet(1);

                                bool firstRow = true;
                                //int columnIndex = 0;
                                foreach (IXLRow row in workSheet.Rows())
                                {
                                    if (firstRow)
                                    {
                                        firstRow = false;
                                    }
                                    else
                                    {
                                        int i = 0;
                                        foreach (IXLCell cell in row.Cells("3"))
                                        {
                                            var phonenumber = cell.Value.ToString();
                                            phoneNumbers.Add(phonenumber);
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                        else if (SenderType == "Others")

                        {
                            phoneNumbers = OthersPhoneNumber.Split(',').ToList();
                        }
                        var fileName = string.Empty;
                        var url = _appSettings.SmsSettings.BaseWhatsAppUrl;
                        var Data = EnglishTranslate + "\n" + TamilTranslate;
                        var AsBase64String = string.Empty;
                        string[] files;
                        var attachementFlag = false;
                        var attachementDocFlag = false;
                        var MsgSendType = "body";
                        if (target != "")
                        {
                            files = Directory.GetFiles(target);
                            fileName = Path.GetFileName(files[0]);
                            var exten = Path.GetExtension(files[0]);
                            byte[] AsBytes = File.ReadAllBytes(files[0]);
                            AsBase64String = Convert.ToBase64String(AsBytes);

                            if (exten == ".jpg" || exten == ".jpeg" || exten == ".gif" || exten == ".png" || exten == ".webp" || exten == ".bmp")
                            {
                                attachementFlag = true;
                                url = "https://api.ultramsg.com/instance76563/messages/image";
                                MsgSendType = "image";
                            }
                            else
                            {
                                attachementDocFlag = true;
                                url = "https://api.ultramsg.com/instance76563/messages/document";
                                MsgSendType = "document";

                            }
                        }
                        var list = phoneNumbers.Select((number, index) => new { number, index }).GroupBy(x => x.index / 500)
                           .Select(g => g.Select(x => x.number).ToList()).ToList();

                        foreach (var item in list)
                        {

                            var client = new RestClient(url);
                            var request = new RestRequest(url, Method.Post);

                            string value = string.Join(",", (item.Select(obj =>

                                "+91" + obj
                            ).ToList()));

                            request.AddHeader("content-type", "application/x-www-form-urlencoded");
                            request.AddParameter("token", "x0p8y2yyxqzz45dg", ParameterType.GetOrPost);
                            request.AddParameter("to", value, ParameterType.GetOrPost);

                            if (attachementFlag || attachementDocFlag)
                            {
                                if (attachementDocFlag)
                                {
                                    request.AddParameter("filename", fileName);
                                }
                                request.AddParameter("caption", Data, ParameterType.GetOrPost);
                                request.AddParameter(MsgSendType, AsBase64String, ParameterType.GetOrPost);
                            }
                            else
                            {
                                request.AddParameter("body", Data, ParameterType.GetOrPost);
                            }

                            request.AddParameter("priority", "5");


                            RestResponse response = client.Execute(request);


                        }

                    }
                    return "Success";

                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }

        }
        public Task<List<StudentMark>> GetStudentMarkByIdDetails(int studentId)
        {
            var spName = ConstantSPnames.SP_GETSTUDENTMARKBYID;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentMark>(spName, new
            {
                StudentId = studentId,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public async Task<List<StudentAttendanceModel>> GetAttendanceByIdDetails(int studentId, int month, int year)
        {
            var spName = ConstantSPnames.SP_GETATTENDANCEBYID;
            //return Task.Factory.StartNew(() => _db.Connection.Query<StudentAttendanceModel>(spName, new
            //{
            //    StudentId = studentId,
            //}, commandType: CommandType.StoredProcedure).ToList());
            using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
            sqlconnection.Open();
            SqlCommand command = new SqlCommand(spName, sqlconnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@StudentId", studentId));
            command.Parameters.Add(new SqlParameter("@Month", month));
            command.Parameters.Add(new SqlParameter("@Year", year));

            //_db.Connection.Open();
            using (var result = command.ExecuteReader())
            {
                var entities = new List<StudentAttendanceModel>();
                while (result.Read())
                {
                    var attendanceRecord = new StudentAttendanceModel
                    {
                        StudentId = result.GetInt64(result.GetOrdinal("StudentId")),
                        StudentName = result.GetString(result.GetOrdinal("StudentName")),
                        Sem = result.GetString(result.GetOrdinal("Sem")),
                        Year = result.GetString(result.GetOrdinal("Year")),
                        AttendanceRecords = new Dictionary<string, string>()
                    };

                    // Handle dynamic columns
                    for (int i = 5; i < result.FieldCount; i++) // Start from 5 assuming the first 4 columns are fixed
                    {
                        attendanceRecord.AttendanceRecords.Add(result.GetName(i), result.IsDBNull(i) ? "N" : result.GetString(i));
                    }

                    entities.Add(attendanceRecord);
                }
                //  _db.Connection.Close();
                return entities;
            }
        }

        public Task<List<AssignmentModel>> GetAllAssignmentDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLASSIGNMENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName,
                new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AssignmentModel>> InsertAssignmentDetails(AssignmentModel assignmentModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTASSIGNMENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName, new
            {
                DepartmentId = assignmentModel.DepartmentId,
                Year = assignmentModel.Year,
                Sem = assignmentModel.Sem,
                Section = assignmentModel.Section,
                SubjectId = assignmentModel.SubjectId,
                FacultyId = assignmentModel.FacultyId,
                DueDate = assignmentModel.DueDate,
                Title = assignmentModel.Title,
                Description = assignmentModel.Description,
                CreatedBy = assignmentModel.CreatedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AssignmentModel>> UpdateAssignmentDetails(AssignmentModel assignmentModel)
        {
            var spName = ConstantSPnames.SP_UPDATEASSIGNMENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName, new
            {
                Id = assignmentModel.Id,
                DepartmentId = assignmentModel.DepartmentId,
                Year = assignmentModel.Year,
                Sem = assignmentModel.Sem,
                Section = assignmentModel.Section,
                SubjectId = assignmentModel.SubjectId,
                FacultyId = assignmentModel.FacultyId,
                DueDate = assignmentModel.DueDate,
                Title = assignmentModel.Title,
                Description = assignmentModel.Description,
                ModifiedBy = assignmentModel.ModifiedBy
            },
         commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AssignmentModel>> DeleteAssignmentDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETEASSIGNMENTFORM;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<AssignmentModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<AssignmentModel>> GetAllAssignmentByStudentDetails(int studentId)
        {
            var spName = ConstantSPnames.SP_GETALLASSIGNMENTBYSTUDENT;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName,
                new { studentId = studentId }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<TimetableModel>> GetAllTimetableDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLTIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName,
                new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> InsertTimetableDetails(TimetableModel timetableModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTTIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName, new
            {
                DepartmentId = timetableModel.DepartmentId,
                Day = timetableModel.Day,
                Year = timetableModel.Year,
                Sem = timetableModel.Sem,
                Section = timetableModel.Section,
                HallNo = timetableModel.HallNo,
                WithEffectFrom = timetableModel.WithEffectFrom,
                Hour1 = timetableModel.Hour1,
                Hour2 = timetableModel.Hour2,
                Hour3 = timetableModel.Hour3,
                Hour4 = timetableModel.Hour4,
                Hour5 = timetableModel.Hour5,
                Hour6 = timetableModel.Hour6,
                Hour7 = timetableModel.Hour7,
                Hour8 = timetableModel.Hour8,
                Hour9 = timetableModel.Hour9,
                CreatedBy = timetableModel.CreatedBy

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> UpdateTimetableDetails(TimetableModel timetableModel)
        {
            var spName = ConstantSPnames.SP_UPDATETIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName, new
            {
                Id = timetableModel.Id,
                DepartmentId = timetableModel.DepartmentId,
                Day = timetableModel.Day,
                Year = timetableModel.Year,
                Sem = timetableModel.Sem,
                Section = timetableModel.Section,
                HallNo = timetableModel.HallNo,
                WithEffectFrom = timetableModel.WithEffectFrom,
                Hour1 = timetableModel.Hour1,
                Hour2 = timetableModel.Hour2,
                Hour3 = timetableModel.Hour3,
                Hour4 = timetableModel.Hour4,
                Hour5 = timetableModel.Hour5,
                Hour6 = timetableModel.Hour6,
                Hour7 = timetableModel.Hour7,
                Hour8 = timetableModel.Hour8,
                Hour9 = timetableModel.Hour9,
                ModifiedBy = timetableModel.ModifiedBy
            },
         commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> DeleteTimetableDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETETIMETABLE;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<TimetableModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }


        public Task<List<StudentFeedbackModel>> GetAllStudentFeedbackDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLSTUDENTFEEDBACKDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentFeedbackModel>(spName,
                new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public async Task<string> InsertStudentFeedbackDetails(List<StudentFeedbackModel> data)
        {
            var spName = ConstantSPnames.SP_INSERTSTUDENTFEEDBACKDETAILS;
            var sendToDB = new ArrayList();
            foreach (var item in data)
            {
                sendToDB.Add(
                    new
                    {
                        FacultyId = item.FacultyId,
                        StudentRollNo = item.StudentRollNo,
                        Feedback = item.Feedback,
                        IsReadyToSentWhatsapp = item.IsReadyToSentWhatsapp,
                        CreatedBy = item.CreatedBy
                    });

            }

            

                // Use Query to get the inserted data, assuming the stored procedure returns the inserted rows.
                 await Task.Factory.StartNew(() =>
                    _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));

                return "success";
           
        }
        //public Task<int> UpdateStudentFeedbackDetails(StudentFeedbackModel studentFeedbackModel)
        //{
        //    var spName = ConstantSPnames.SP_UPDATESTUDENTFEEDBACKDETAILS;
        //    return Task.Factory.StartNew(() => _db.Connection.Query<StudentFeedbackModel>(spName, new
        //    {
        //        Id = studentFeedbackModel.Id,
        //        FacultyId= studentFeedbackModel.FacultyId,
        //        StudentId= studentFeedbackModel.StudentId,
        //        Feedback= studentFeedbackModel.Feedback,
        //        IsFeedbackSent=studentFeedbackModel.IsFeedbackSent,
        //        ModifiedBy = studentFeedbackModel.ModifiedBy
        //    },
        // commandType: CommandType.StoredProcedure).ToList());
        //}
        public Task<List<StudentFeedbackModel>> DeleteStudentFeedbackDetails(string id)
        {
            var spName = ConstantSPnames.SP_DELETESTUDENTFEEDBACK;
            //  return await connection.QueryFirstOrDefaultAsync<StudentFeedbackModel>(spName, parameters, commandType: CommandType.StoredProcedure);
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<StudentFeedbackModel>(spName, new { Ids = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<NotificationModel>> GetNotificationDetails(int studentId, string role)
        {
            var spName = ConstantSPnames.SP_GETNOTIFICATION;
            //  return await connection.QueryFirstOrDefaultAsync<StudentFeedbackModel>(spName, parameters, commandType: CommandType.StoredProcedure);
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<NotificationModel>(spName, new { studentId = studentId, role = role }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<UserFcmToken>> SaveFcmToken(UserFcmToken model)
        {
            var spName = ConstantSPnames.SP_INSERTUSERFCMTOKENS;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<UserFcmToken>(spName, new { @UserId = model.UserId,@Role=model.Role, @DeviceToken = model.DeviceToken }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<UserFcmToken>> GetUserDeviceToken(int studentId, string role)
        {
            var spName = ConstantSPnames.SP_GETUSERTOKEN;
            //  return await connection.QueryFirstOrDefaultAsync<StudentFeedbackModel>(spName, parameters, commandType: CommandType.StoredProcedure);
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<UserFcmToken>(spName, new { studentId = studentId, role = role }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<NotificationModel>> UpdateNotificationDetails(NotificationModel notificationModel)
        {
            var spName = ConstantSPnames.SP_UPDATENOTIFICATION;
            //  return await connection.QueryFirstOrDefaultAsync<StudentFeedbackModel>(spName, parameters, commandType: CommandType.StoredProcedure);
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<NotificationModel>(spName, new { StudentId = notificationModel.StudentId }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<ContentLibModel>> GetAllContentLibDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETCONTENTLIBDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ContentLibModel>(spName,
                new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ContentLibModel>> InsertContentLibDetails(ContentLibModel contentLibModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTCONTENTLIBDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ContentLibModel>(spName, new
            {


                DepartmentId = contentLibModel.DepartmentId,
                FacultyId = contentLibModel.FacultyId,
                Year = contentLibModel.Year,
                Sem = contentLibModel.Sem,
                Section = contentLibModel.Section,
                Title = contentLibModel.Title,
                Description=contentLibModel.Description,
                ExpiryDate=contentLibModel.ExpiryDate,
                CreatedBy = contentLibModel.CreatedBy,
                CreatedDate = contentLibModel.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ContentLibModel>> UpdateContentLibDetails(ContentLibModel contentLibModel)
        {
            var spName = ConstantSPnames.SP_UPDATECONTENTLIBDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ContentLibModel>(spName, new
            {
                Id = contentLibModel.Id,
                DepartmentId = contentLibModel.DepartmentId,
                Year = contentLibModel.Year,
                Sem= contentLibModel.Sem,
                Section = contentLibModel.Section,
                Title = contentLibModel.Title,
                FacultyId=contentLibModel.FacultyId,
                Description = contentLibModel.Description,
                ExpiryDate = contentLibModel.ExpiryDate,
                ModifiedBy = contentLibModel.ModifiedBy,
                ModifiedDate = contentLibModel.ModifiedDate
            },
        commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ContentLibModel>> DeleteContentLibDetails(int id)
        {
            var spName = ConstantSPnames.SP_DELETECONTENTLIB;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<ContentLibModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<ContentLibModel>> GetAllContentLibByStudentDetails(int studentId)
        {
            var spName = ConstantSPnames.SP_GETALLCONTENTLIBBYSTUDENT;
            return Task.Factory.StartNew(() => _db.Connection.Query<ContentLibModel>(spName,
                new { studentId = studentId }, commandType: CommandType.StoredProcedure).ToList());
        }
      
        public Task<List<BirthdayModel>> GetBirthdayListByRole(string role)
        {
            var spName = ConstantSPnames.SP_GETALLBIRTHDAYLIST;
            return Task.Factory.StartNew(() => _db.Connection.Query<BirthdayModel>(spName,
                new { Role = role }, commandType: CommandType.StoredProcedure).ToList());
        }

        public  Task<List<Feedbacksubject>> checkFeedbackSubmittedAsync(int studentId)
        {
            List<Feedbacksubject> subjectDetails;
            string spToCheckfb = "[sp_CheckFeedbackSubmittedNew]";

            return Task.Factory.StartNew(() => _db.Connection.Query<Feedbacksubject>(spToCheckfb,
              new { @StudentId = studentId }, commandType: CommandType.StoredProcedure).ToList());

   
        }
        public  Task<List<Feedbacksubject>> getSubFacultyList(int studentId)
        {

            List<Feedbacksubject> subjectDetails;
            string spToGetfb = "sp_GetSubFacultyFeedbackNew";
            return Task.Factory.StartNew(() => _db.Connection.Query<Feedbacksubject>(spToGetfb,
              new { @StudentId = studentId }, commandType: CommandType.StoredProcedure).ToList());

        }
        public async Task<Feedback> getFeedbackDetails(int studentId, int subjectId, int facultyId)
        {
            List<Feedback> fb;
            int a;
            string GetFeedback = "[sp_GetFeedbackFormNew]";
            string GetInsertedFeedback = "[sp_GetInsertedFeedback]";

            using (SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {

                var parameters = new DynamicParameters();
                parameters.Add("@StudentId", studentId, DbType.Int32);
                parameters.Add("@SubjectId", subjectId, DbType.Int32);
                parameters.Add("@FacultyId", facultyId, DbType.Int32);
                fb = con.Query<Feedback>(GetFeedback, parameters, commandType: CommandType.StoredProcedure)
                            .ToList();

            }

            List<QnsModel> _questions = await getQuestions();
            Feedback feedback = fb.FirstOrDefault();

            if (!string.IsNullOrEmpty(feedback.FeedbackData))
            {
                try
                {
                    // Deserialize FeedbackData JSON into Questions list
                    feedback.Questions = JsonConvert.DeserializeObject<List<QnsModel>>(feedback.FeedbackData);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing FeedbackData for Feedback ID {feedback.Id}: {ex.Message}");
                    feedback.Questions = new List<QnsModel>(); // Assign empty list on error
                }
            }
            else
            {
                List<QnsModel> qns = new List<QnsModel>();
                qns = _questions;

                feedback.Questions = qns; // Assign empty list if FeedbackData is null/empty
            }




            return feedback;
        }
        public async Task<List<QnsModel>> getQuestions()
        {
            List<QnsModel> _questions = new List<QnsModel>();
            string selectSQL = "[sp_GetQuestionsMaster]";

            using (SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                await con.OpenAsync(); // Open the connection asynchronously

                using (SqlCommand cmd = new SqlCommand(selectSQL, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader GrReader = await cmd.ExecuteReaderAsync()) // Execute reader asynchronously
                    {
                        while (await GrReader.ReadAsync()) // Read asynchronously
                        {
                            _questions.Add(new QnsModel
                            {
                                QnsId = Convert.ToInt32(GrReader["QnsId"]),
                                QnsCode = Convert.ToString(GrReader["QnsCode"]),
                                QnsDescription = Convert.ToString(GrReader["QnsDescription"]),
                            });
                        }
                    }
                }
            }
            return _questions;
        }

        public async Task<string> InsertFeedBackDetails(Feedback feedback)
        {
            string selectSQL = "[sp_InsertFeedback]";
            DateTime now = DateTime.Now;

            try
            {
                using (SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    SqlCommand cmd = new SqlCommand(selectSQL, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", feedback.Id);
                    cmd.Parameters.AddWithValue("@StudentId", feedback.StudentId);
                    cmd.Parameters.AddWithValue("@SubjectId", feedback.SubjectId);
                    cmd.Parameters.AddWithValue("@Sem", feedback.Sem);
                    cmd.Parameters.AddWithValue("@Year", feedback.Year);
                    cmd.Parameters.AddWithValue("@Section", feedback.Section);
                    cmd.Parameters.AddWithValue("@FacultyId", feedback.facultyId);
                    cmd.Parameters.AddWithValue("@DepartmentId", feedback.DepartmentId);
                    cmd.Parameters.AddWithValue("@FeedbackData", JsonConvert.SerializeObject(feedback.Questions));
                    cmd.Parameters.AddWithValue("@FeedbackReviewScore", feedback.FeedbackReviewScore == null ? "0" : feedback.FeedbackReviewScore);
                    cmd.Parameters.AddWithValue("@StudentNote", feedback.StudentNote == null ? "" : feedback.StudentNote);
                    cmd.Parameters.AddWithValue("@IsCompleted", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", feedback.StudentName);
                    cmd.Parameters.AddWithValue("@CreatedDate", now);
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                return ("Success");
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public async Task<string> UpdateSubmitStatus(int studentId)
        {
            string selectSQL = "[sp_UpdateFeedbackSubmit]";
            DateTime now = DateTime.Now;

            try
            {
                using (SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    SqlCommand cmd = new SqlCommand(selectSQL, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                return ("Success");
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public Task<List<AcademicCalender>> GetAllAcademicCalender(string role, int year, string sem)
        {
            var spName = ConstantSPnames.SP_GETACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName,
                new { Role= role,Year = year,Sem=sem }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> InsertAcademicCalender(AcademicCalender academicCalender)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName, new
            {


                Year= academicCalender.Year,
                Sem= academicCalender.Sem,
                AcademicActivities=academicCalender.AcademicActivities,
                StartDate=academicCalender.StartDate,
                EndDate =academicCalender.EndDate,
                CreatedBy = academicCalender.CreatedBy,
                CreatedDate = academicCalender.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> UpdateAcademicCalender(AcademicCalender academicCalender)
        {
            var spName = ConstantSPnames.SP_UPDATEACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName, new
            {
                SNo = academicCalender.SNo,
                Year = academicCalender.Year,
                Sem = academicCalender.Sem,
                AcademicActivities = academicCalender.AcademicActivities,
                StartDate = academicCalender.StartDate,
                EndDate = academicCalender.EndDate,
                ModifiedBy = academicCalender.ModifiedBy,
                ModifiedDate = academicCalender.ModifiedDate
            },
        commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> DeleteAcademicCalender(int SNo)
        {
            var spName = ConstantSPnames.SP_DELETEACADEMICCALENDER;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<AcademicCalender>(spName, new { SNo = SNo }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<InfoGaloreModel>> GetAllInfoGalore(int? id)
        {
            var spName = ConstantSPnames.SP_GETINFOGALORE;
            return Task.Factory.StartNew(() => _db.Connection.Query<InfoGaloreModel>(spName,
                new { @Id=id}, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InfoGaloreModel>> InsertInfoGalore(InfoGaloreModel infoGaloreModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTINFOGALORE;
            return Task.Factory.StartNew(() => _db.Connection.Query<InfoGaloreModel>(spName, new
            {


                InfoFileName =infoGaloreModel.InfoFileName,
                ValidDate =infoGaloreModel.ValidDate,
                CreatedBy = infoGaloreModel.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InfoGaloreModel>> UpdateInfoGalore(int id,string target)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_UPDATEINFOGALORE;
            return Task.Factory.StartNew(() => _db.Connection.Query<InfoGaloreModel>(spName, new
            {
                @Id=id,@InfoFilePath =target
            }, commandType: CommandType.StoredProcedure).ToList());
        }
    }
}
