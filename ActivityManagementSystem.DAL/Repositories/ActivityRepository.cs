using ActivityManagementSystem.DAL.Constants;
using ActivityManagementSystem.DAL.Infrastructure;
using ActivityManagementSystem.DAL.Interfaces;
using ActivityManagementSystem.Domain.AppSettings;
using ActivityManagementSystem.Domain.Models.Activity;
using ClosedXML.Excel;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using System.Xml;

using RestSharp;


using System.Globalization;

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
        private string NotFound(string v)
        {
            throw new NotImplementedException();
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


        public Task<List<StudentDropdown>> GetStudentByName(string studentName)
        {
            var spName = ConstantSPnames.SP_GETSTUDENTBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentDropdown>(spName, new
            {
                StudentName = studentName


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


        public async Task<string> DeleteRole(int id)
        {
            var spName = ConstantSPnames.SP_DELETEROLEMASTER;

            try
            {
                await using (SqlConnection sqlconnection =
                    new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
                {
                    await sqlconnection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(spName, sqlconnection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@roleId", SqlDbType.Int).Value = id;

                        await command.ExecuteNonQueryAsync();
                    }

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                // You can log ex here before returning if needed
                return $"Error: {ex.Message}";
            }
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
                DOJ=studentDetails.DOJ,
                DOL =studentDetails.DOL,
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

                Gardian3Name = studentDetails.Gardian3Name,
                Gardian3MobileNumber = studentDetails.Gardian3MobileNumber,
                Gardian3Photo = studentDetails.Gardian3Photo,

                Gardian4Name = studentDetails.Gardian4Name,
                Gardian4MobileNumber = studentDetails.Gardian4MobileNumber,
                Gardian4Photo = studentDetails.Gardian4Photo,


                CreatedBy = studentDetails.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }


        public Task<List<HouseModel>> InsertHouseDetails(HouseModel house)
        {
            var spName = ConstantSPnames.SP_INSERTHOUSE;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseModel>(spName, new
            {
                Name=house.Name,
                Is_Active = house.Is_Active,
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
                DOJ = studentDetails.DOJ,
                DOL = studentDetails.DOL,
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
                Gardian3Name = studentDetails.Gardian3Name,
                Gardian3MobileNumber = studentDetails.Gardian3MobileNumber,
                Gardian3Photo = studentDetails.Gardian3Photo,

                Gardian4Name = studentDetails.Gardian4Name,
                Gardian4MobileNumber = studentDetails.Gardian4MobileNumber,
                Gardian4Photo = studentDetails.Gardian4Photo,
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

        public Task<List<FacultyModel>> InsertFacultyDetails(FacultyModel facultyDetails)
        {
            var spName = ConstantSPnames.SP_INSERTFACULTY;

            return Task.Factory.StartNew(() => _db.Connection.Query<FacultyModel>(spName, new
            {
                UserName = facultyDetails.UserName,
                Password = facultyDetails.Password,
                RoleId = facultyDetails.RoleId,
                FacultyId = facultyDetails.FacultyId,
                Faculty_FirstName = facultyDetails.Faculty_FirstName,
                Faculty_MiddleName = facultyDetails.Faculty_MiddleName,
                Faculty_LastName = facultyDetails.Faculty_LastName,
                Gender = facultyDetails.Gender,
                DOB = facultyDetails.DOB,
                FacultyMobileNo_1 = facultyDetails.FacultyMobileNo_1,
                FacultyMobileNo_2 = facultyDetails.FacultyMobileNo_2,
                Email = facultyDetails.Email,
                FilePath = facultyDetails.FilePath,
                FileNames = facultyDetails.FileNames,
                BloodGroup = facultyDetails.BloodGroup,
                Address = facultyDetails.Address,
                CreatedBy = facultyDetails.CreatedBy,
                CreatedDate = facultyDetails.CreatedDate,
                ModifiedBy = facultyDetails.ModifiedBy,
                ModifiedDate = facultyDetails.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
              
           




       
        public async Task<List<FacultyModel>> UpdateFacultyDetails(FacultyModel facultyDetails)
        {
            var spName = ConstantSPnames.SP_UPDATEFACULTY;

            return await Task.Factory.StartNew(() => _db.Connection.Query<FacultyModel>(spName, new
            {
                Id=facultyDetails.Id,
                UserName = facultyDetails.UserName,
                Password = facultyDetails.Password,
                RoleId = facultyDetails.RoleId,
                FacultyId = facultyDetails.FacultyId,
                Faculty_FirstName = facultyDetails.Faculty_FirstName,
                Faculty_MiddleName = facultyDetails.Faculty_MiddleName,
                Faculty_LastName = facultyDetails.Faculty_LastName,
                Gender = facultyDetails.Gender,
                DOB = facultyDetails.DOB,
                FacultyMobileNo_1 = facultyDetails.FacultyMobileNo_1,
                FacultyMobileNo_2 = facultyDetails.FacultyMobileNo_2,
                Email = facultyDetails.Email,
                FilePath = facultyDetails.FilePath,
                FileNames = facultyDetails.FileNames,
                BloodGroup = facultyDetails.BloodGroup,
                Address = facultyDetails.Address,
                CreatedBy = facultyDetails.CreatedBy,
                CreatedDate = facultyDetails.CreatedDate,
                ModifiedBy = facultyDetails.ModifiedBy,
                ModifiedDate = facultyDetails.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
           
            

        }

        public Task<List<HouseModel>> UpdateHouseDetails(HouseModel house)
        {
            var spName = ConstantSPnames.SP_UPDATEHOUSE;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseModel>(spName, new
            {
                Id = house.Id,
                 Name=house.Name,
                Is_Active = house.Is_Active,
                ModifiedBy = house.ModifiedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<HouseModel>> DeleteHouseDetails(int id)
        {
            
                var spName = ConstantSPnames.SP_DELETEHOUSE;

                return Task.Factory.StartNew(() =>
                _db.Connection.Query<HouseModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        
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
                Grade = subject.Grade,
                CreatedBy = subject.CreatedBy               

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
                Grade = subject.Grade,
                ModifiedBy = subject.ModifiedBy              

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

        public Task<List<BatchStudMappingModel>> GetAllSectionStudMappings(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLSECTIONSTUDMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchStudMappingModel>(spName, new
            {
                Id = id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<int> InsertSectionStudMappings(List<BatchStudMappingModel> data)
        {
            var spName = ConstantSPnames.SP_INSERTSECTIONSTUDMAP;
            var sendToDB = new ArrayList();
            foreach (var item in data)
            {
                sendToDB.Add(
                    new
                    {
                        SectionId = item.SectionId,
                        StudentId = item.StudentId,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
         
        }

        public Task<int> UpdateSectionStudMapping(List<BatchStudMappingModel> model)
        {
            var spName = ConstantSPnames.SP_UPDATESECTIONSTUDMAP;
            var sendToDB = new ArrayList();

            string sProc = ConstantSPnames.SP_UPDATESECTIONSTUDACTIVEMAP;
            var rowsUpdated = _db.Connection.Execute(sProc,
                new { SectionId = model.FirstOrDefault(x => x.SectionId != 0).SectionId },
                commandType: CommandType.StoredProcedure);
            foreach (var item in model)
            {
                sendToDB.Add(
                    new
                    {
                        Id = item.Id,
                        SectionId = item.SectionId,
                        StudentId = item.StudentId,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate
                    });

            }

            return Task.Factory.StartNew(() =>
                _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure));
        }

        public Task<int> DeleteSectionStudMapping(int[] ids, int batchId)
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

            var spName1 = ConstantSPnames.SP_DELETESECTIONSTUDMAP;
            var spName = ConstantSPnames.SP_UPDATESECTIONSTUDACTIVEMAP;
            //  _db.Connection.Execute(spName, sendToDB.ToArray(), commandType: CommandType.StoredProcedure))
            var rowsUpdated = _db.Connection.Execute(spName, new { SectionId = batchId },
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


        public async Task<string> bulkuploadfaculty(DataTable target)
        {

            try
            {
                var spName = ConstantSPnames.SP_BULKFACULTYUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync(); // Await this!

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@FacultyTable", SqlDbType.Structured).Value = target;

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

        public async Task<string> bulkuploadsubject(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKSUBJECTUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync();

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@SubjectTable", SqlDbType.Structured).Value = target;

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
        public async Task<string> bulkuploadtimetable(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKTIMETABLEUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync();

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@TimetableTable", SqlDbType.Structured).Value = target;

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

        public async Task<string> bulkuploadholidaycalendar(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKHOLIDAYUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync();

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@HolidayTable", SqlDbType.Structured).Value = target;

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
        public async Task<string> bulkuploadacademiccalendar(DataTable target)
        {
            try
            {
                var spName = ConstantSPnames.SP_BULKACADEMICUPLOAD;
                using SqlConnection sqlConnection = new(_db.Connection.ConnectionString);
                await sqlConnection.OpenAsync();

                using SqlCommand command = new(spName, sqlConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@AcademicCalendarTable", SqlDbType.Structured).Value = target;

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

                command.Parameters.Add("@xml", SqlDbType.VarChar).Value = xml;
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
                Date = attendance.Date,
                IsPresent = attendance.IsPresent,
                ModifiedBy = attendance.ModifiedBy,
                ModifiedDate = attendance.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }

    

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

        public Task<List<BatchSubjectFacultyModel>> GetAllBatchSubMapping(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLBATCHSUBMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchSubjectFacultyModel>(spName, new
            {
                Id = id

            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<BatchSubjectFacultyModel>> InsertBatchSubMappings(BatchSubjectFacultyModel data)
        {
            var spName = ConstantSPnames.SP_INSERTBATCHSUBMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchSubjectFacultyModel>(spName, new
            {
                Name = data.SectionName,
                SectionId = data.sectionID,
                SubjectId = data.SubjectID,
                FacultyID = data.FacultyID,
                CreatedBy = data.CreatedBy,
                CreatedDate = data.CreatedDate

            }, commandType: CommandType.StoredProcedure).ToList());
           
        }

        public Task<List<BatchSubjectFacultyModel>> UpdateBatchSubMapping(BatchSubjectFacultyModel data)

        {
            var spName = ConstantSPnames.SP_UPDATEBATCHSUBMAP;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchSubjectFacultyModel>(spName, new
            {
                Id = data.Id,
                Name = data.SectionName,
                SectionId = data.sectionID,
                SubjectId = data.SubjectID,
                FacultyID = data.FacultyID,
                ModifiedBy = data.ModifiedBy,
                ModifiedDate = data.ModifiedDate

            }, commandType: CommandType.StoredProcedure).ToList());

        }
       

       
            public string DeleteBatchSubMapping(int id)
            {
                try
                {

                    var spName = ConstantSPnames.SP_DELETEBATCHSUBMAP;

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

}
        

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

        public string generateMonthlyAttendancereport(int startMonth, int startYear, int endMonth, int endYear, int sectionId,string grade,string section)
        {
            var spMonthwiseAtt = ConstantSPnames.SP_MonthwiseAttendance;
            string strfilepath = "";
            // DataTable dtCloned = new DataTable();

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
                        objCmd.Parameters.Add("@StartMonth", SqlDbType.Int).Value = startMonth;
                        objCmd.Parameters.Add("@StartYear", SqlDbType.Int).Value = startYear;
                        objCmd.Parameters.Add("@EndMonth", SqlDbType.Int).Value = endMonth;
                        objCmd.Parameters.Add("@EndYear", SqlDbType.Int).Value = endYear;
                        objCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = sectionId;
                       
                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        var dataTable = ds.Tables[0];
                        // Create a new DataTable for the transformed data
                        if (dataTable.Rows.Count == 0)
                            return NotFound("No attendance records found.");
                         strfilepath = GenerateExcel(dataTable, "Cumulative Report- Grade:" + grade + "/ Section: " + section);
                    }

                }
                return strfilepath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex.InnerException.ToString());

                return ex.Message;
            }
        }
        private string GenerateExcel(DataTable dataTable, string reportname)
        {
            // Define folder and file paths
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string filePath = Path.Combine(folderPath, "Report.xlsx");

            // Ensure the directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Delete the old file (if it exists)
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Report");

                // Add title row
                worksheet.Cell(1, 1).Value = "Sona Valliappa Public School";
                worksheet.Range(1, 1, 1, dataTable.Columns.Count).Merge();
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add report name row
                worksheet.Cell(2, 1).Value = reportname;
                worksheet.Range(2, 1, 2, dataTable.Columns.Count).Merge();
                worksheet.Cell(2, 1).Style.Font.Bold = true;
                worksheet.Cell(2, 1).Style.Font.FontSize = 14;
                worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add header row
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    worksheet.Cell(3, col + 1).Value = dataTable.Columns[col].ColumnName;
                    worksheet.Cell(3, col + 1).Style.Font.Bold = true;
                }

                // Add data rows
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cell(row + 4, col + 1).Value = dataTable.Rows[row][col].ToString();
                    }
                }

                // Auto adjust column width
                worksheet.Columns().AdjustToContents();

                // Save the file
                workbook.SaveAs(filePath);
            }

            return filePath; // Return the full file path
        }
        public string generateExcelList(string role)
        {
            var spMonthwiseAtt = role == "Student"? ConstantSPnames.SP_GETALLSTUDENTDETAILWITHSECTION  : ConstantSPnames.SP_GETALLFACULTYLIST;
            string strfilepath = "";
            // DataTable dtCloned = new DataTable();

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

                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        var dataTable = ds.Tables[0];
                        // Create a new DataTable for the transformed data
                        if (dataTable.Rows.Count == 0)
                            return NotFound("No attendance records found.");
                        strfilepath = GenerateExcel(dataTable, role.ToUpper() +" List") ;
                    }

                }
                return strfilepath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex.InnerException.ToString());

                return ex.Message;
            }
        }
        public string generateDailyAttendancereport(int month, int year, int sectionId, string grade, string section)
        {
            var spMonthwiseAtt = ConstantSPnames.SP_MonthwiseDynamicAttendance;
            string strfilepath = "";
            // DataTable dtCloned = new DataTable();

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
                        objCmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        objCmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                        
                        objCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = sectionId;

                        objCmd.CommandTimeout = 100000;
                        //SqlParameter activeStudentCountParam = new SqlParameter("@ActiveStudentCount", SqlDbType.Int);
                        //activeStudentCountParam.Direction = ParameterDirection.Output;
                        //objCmd.Parameters.Add(activeStudentCountParam);

                        da.Fill(ds);
                        //int activeStudentCount = (int)activeStudentCountParam.Value;
                        //var dataTable = ds.Tables[0];
                        // Create a new DataTable for the transformed data
                        //if (dataTable.Rows.Count == 0)
                        if(ds.Tables.Count ==0)
                            return NotFound("No attendance records found.");
                        strfilepath = GenerateExcelSheet(ds.Tables[0],
        ds.Tables[1],
        ds.Tables[2],
        ds.Tables[3], "Daily Attendance Report- Grade:" + grade + "/ Section: " + section,month);
                    }

                }
                return strfilepath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex.InnerException.ToString());

                return ex.Message;
            }
        }
        public string generateAttendanceCumulativereport(int startYear, int endyear, int sectionId)
        {
            var spMonthwiseAtt = ConstantSPnames.SP_CumulativeAttendance;
            string strfilepath = "";
            // DataTable dtCloned = new DataTable();

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
                        objCmd.Parameters.Add("@StartYear", SqlDbType.Int).Value = startYear;
                        objCmd.Parameters.Add("@EndYear", SqlDbType.Int).Value = endyear;

                        objCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = sectionId;

                        objCmd.CommandTimeout = 100000;
                        da.Fill(ds);
                        var dataTable = ds.Tables[0];
                        // Create a new DataTable for the transformed data
                        if (dataTable.Rows.Count == 0)
                            return NotFound("No attendance records found.");
                        strfilepath = GenerateExcelReport(dataTable, "Attendance Cumulative Report");
                    }

                }
                return strfilepath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_logger.LogError(ex.InnerException.ToString());

                return ex.Message;
            }
        }
        private string GenerateExcelReport(DataTable dataTable, string reportname)
        {
            // Define folder and file paths
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string filePath = Path.Combine(folderPath, "CUMReport.xlsx");

            // Ensure directory exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cumulative Attendance Report");

                // Title Row
                worksheet.Cell(1, 1).Value = "Sona Valliappa Public School";
                worksheet.Range(1, 1, 1, dataTable.Columns.Count).Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Border.OutsideBorder = XLBorderStyleValues.Thick;

                // Report Name Row
                worksheet.Cell(2, 1).Value = reportname;
                worksheet.Range(2, 1, 2, dataTable.Columns.Count).Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Border.OutsideBorder = XLBorderStyleValues.Thick;

                int headerRow = 3;
                int dataStartRow = 4;

                // Write headers
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    var headerCell = worksheet.Cell(headerRow, col + 1);
                    headerCell.Value = dataTable.Columns[col].ColumnName;
                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Write data rows
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        var value = dataTable.Rows[row][col];
                        var cell = worksheet.Cell(row + dataStartRow, col + 1);

                        if (value == DBNull.Value)
                            cell.Value = ""; // or "N/A"
                        else
                            cell.Value = value?.ToString() ?? string.Empty;


                    }
                }

                int totalColumns = dataTable.Columns.Count;
                int dataEndRow = dataStartRow + dataTable.Rows.Count - 1;

                // Apply table border
                var fullRange = worksheet.Range(headerRow, 1, dataEndRow, totalColumns);
                fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Apply percentage format
                string[] percentCols = { "% for L.T.", "% for S.T.", "Overall %" };
                foreach (string colName in percentCols)
                {
                    if (dataTable.Columns.Contains(colName))
                    {
                        int colIndex = dataTable.Columns.IndexOf(colName) + 1;
                        var percentRange = worksheet.Range(dataStartRow, colIndex, dataEndRow, colIndex);
                        percentRange.Style.NumberFormat.Format = "0.00";
                        // Optional: highlight low attendance
                        percentRange.AddConditionalFormat()
                            .WhenLessThan(75)
                            .Fill.SetBackgroundColor(XLColor.LightPink);
                    }
                }

                // Signature section
                int signatureRowStart = dataEndRow + 1;
                int signatureRowEnd = signatureRowStart + 2;
                var signatureRange = worksheet.Range(signatureRowStart, 1, signatureRowEnd, totalColumns);
                signatureRange.Merge();
                signatureRange.Value = "Signature";
                signatureRange.Style
                    .Font.SetBold()
                    .Font.SetFontSize(15)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Bottom)
                    .Border.OutsideBorder = XLBorderStyleValues.Thick;

                // Outer border
                var fullOutlineRange = worksheet.Range(1, 1, signatureRowEnd, totalColumns);
                fullOutlineRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                // Auto fit
                worksheet.Columns().AdjustToContents();

                // Save the file
                workbook.SaveAs(filePath);
            }

            return filePath;
        }



        private string GenerateExcelSheet(DataTable dataTable, DataTable table2, DataTable table3, DataTable table4, string reportname,int month)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string filePath = Path.Combine(folderPath, "Report.xlsx");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (File.Exists(filePath))
                File.Delete(filePath);
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Report");
                int totalColumns = dataTable.Columns.Count;
                int totalRows = dataTable.Rows.Count;

                // 1. School Title
                // 1. School Title with separate thick border
                worksheet.Cell(1, 1).Value = "Sona Valliappa Public School";
                var schoolTitleRange = worksheet.Range(1, 1, 1, totalColumns);
                schoolTitleRange.Merge();
                schoolTitleRange.Style.Font.SetBold()
                                  .Font.SetFontSize(14)
                                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                  .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Apply full thick border to school title
                schoolTitleRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                schoolTitleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                schoolTitleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                schoolTitleRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;


                // 2. Report Title with separate thick border
                worksheet.Cell(2, 1).Value = reportname;
                var reportTitleRange = worksheet.Range(2, 1, 2, totalColumns);
                reportTitleRange.Merge();
                reportTitleRange.Style.Font.SetBold()
                                   .Font.SetFontSize(13)
                                   .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                   .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Apply full thick border to report title
                reportTitleRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                reportTitleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                reportTitleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                reportTitleRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                // 3. Column Headers
                int headerRow = 3;
                for (int col = 0; col < totalColumns; col++)
                {
                    worksheet.Cell(headerRow, col + 1).Value = dataTable.Columns[col].ColumnName;
                    worksheet.Cell(headerRow, col + 1).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                // 4. Attendance Data Rows
                int dataStartRow = headerRow + 1;

                for (int row = 0; row < totalRows; row++)
                {
                    for (int col = 0; col < totalColumns; col++)
                    {
                        worksheet.Cell(dataStartRow + row, col + 1).Value = dataTable.Rows[row][col]?.ToString();
                        worksheet.Cell(dataStartRow + row, col + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    }
                }

                int dataEndRow = dataStartRow + totalRows - 1;
                

                // 5. Number Present Daily & Initials of Teacher (below data)
                int summaryRowStart = dataEndRow + 1;

                // "Number Present Daily" - Column B
                worksheet.Cell(summaryRowStart, 1).Value = "Number Present Daily";
                worksheet.Range(summaryRowStart, 1, summaryRowStart + 1, 2).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(summaryRowStart, 3).Value = "M";
                worksheet.Cell(summaryRowStart + 1, 3).Value = "E";
                // Add days as headers (1 to 31) starting from column 4
                int totalDays = table2.Rows.Count;
                //int totalDaysColumn = table2.Column.Count;
                int dayColumnStart = 4;
                /*for (int i = 0; i < totalDays; i++)
                {
                    int day = Convert.ToInt32(table2.Rows[i]["Day"]); // Get the day value from table2
                    worksheet.Cell(summaryRowStart, dayColumnStart + i).Value = day.ToString(); // Day header
                    worksheet.Cell(summaryRowStart, dayColumnStart + i).Style.Font.SetBold();
                }*/

                // Populate Students Present based on Day column from table2
                for (int i = 0; i < totalDays; i++)
                {
                    int studentsPresent = Convert.ToInt32(table2.Rows[i]["StudentsPresent"]); // Get Students Present value

                    worksheet.Cell(summaryRowStart, dayColumnStart + i).Value = studentsPresent;       // Morning (moved up by 1 row)
                    worksheet.Cell(summaryRowStart + 1, dayColumnStart + i).Value = studentsPresent;   // Evening (moved up by 1 row)
                }

                // Adjust styling and formatting for the new range
                worksheet.Range(summaryRowStart, dayColumnStart, summaryRowStart + 1, dayColumnStart + totalDays - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(summaryRowStart, dayColumnStart, summaryRowStart + 1, dayColumnStart + totalDays - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(summaryRowStart, dayColumnStart, summaryRowStart + 1, dayColumnStart + totalDays - 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                summaryRowStart = summaryRowStart+2;
                worksheet.Cell(summaryRowStart, 1).Value = "Initials Of Teachers";
                worksheet.Range(summaryRowStart, 1, summaryRowStart + 1, 2).Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(summaryRowStart, 3).Value = "M";
                worksheet.Cell(summaryRowStart + 1, 3).Value = "E";


                // Calculate total students
                int totalStudents = 0;
                if (table2.Rows.Count > 0 && table2.Columns.Contains("TotalStudents"))
                {
                    totalStudents = Convert.ToInt32(table2.Rows[0]["TotalStudents"]);
                }

                int newJoinedCount = 0;
                if (table3.Columns.Contains("StudentCount"))
                {
                    foreach (DataRow row in table3.Rows)
                    {
                        //newJoinedCount += Convert.ToInt32(row["StudentCount"]);
                        if (int.TryParse(row["StudentCount"]?.ToString(), out int studentCount))
                        {
                            newJoinedCount += studentCount;
                        }
                    }
                }

              
                int LeftStudnetCount = 0;
                if (table4.Columns.Contains("StudentCount"))
                {
                    foreach (DataRow row in table4.Rows)
                    {
                        //newJoinedCount += Convert.ToInt32(row["StudentCount"]);
                        if (int.TryParse(row["StudentCount"]?.ToString(), out int studentCount))
                        {
                            LeftStudnetCount += studentCount;
                        }
                    }
                }
                int totalEnrolled = totalStudents + newJoinedCount -LeftStudnetCount;
                // Calculate total working days
                /*int totalWorkingDays = 0;
                if (table2.Rows.Count > 0 && table2.Columns.Contains("StudentsPresent"))
                {
                    foreach (DataRow row in table2.Rows)
                    {
                        if (int.TryParse(row["StudentsPresent"]?.ToString(), out int studentsPresent) && studentsPresent > 0)
                        {
                            totalWorkingDays++;
                        }
                    }
                }
                */
                int dayColumn = 0;
                int dayColumnEnd = 0;

                // Dynamically find the range of day columns (1-31)
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    if (int.TryParse(dataTable.Columns[col].ColumnName, out int day))
                    {
                        if (dayColumn == 0) dayColumn = col + 1; // Set start column
                        dayColumnEnd = col + 1; // Continuously update the end column
                    }
                }

                // Calculate total working days
                int totalWorkingDays = 0;

                // Loop through each day column to determine working days
                for (int colIndex = dayColumn; colIndex <= dayColumnEnd; colIndex++)
                {
                    bool isWorkingDay = false;

                    // Check if any row has either 'P' or 'A' for the current day
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string attendanceMark = row[colIndex - 1]?.ToString()?.Trim().ToUpper();

                        if (attendanceMark == "P" || attendanceMark == "A")
                        {
                            isWorkingDay = true;
                            break; // No need to check further rows for this day
                        }
                    }

                    if (isWorkingDay)
                    {
                        totalWorkingDays++;
                    }
                }

                // Calculate total present count
                int totalPresentCount = 0;
                for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
                {
                    for (int colIndex = dayColumn - 1; colIndex < dayColumnEnd; colIndex++)
                    {
                        string attendanceMark = dataTable.Rows[rowIndex][colIndex]?.ToString()?.Trim().ToUpper();
                        if (attendanceMark == "P")
                        {
                            totalPresentCount++;
                        }
                    }
                }
                double averageNoOnRoll = totalStudents > 0 ? (double)totalPresentCount / totalStudents : 0;
                // Calculate average attendance
                double averageAttendance = totalWorkingDays > 0 ? (double)totalPresentCount / totalWorkingDays : 0;

                // 6. Monthly Summary Rows
                int rollSummaryStart = summaryRowStart + 2;
                worksheet.Cell(rollSummaryStart, 1).Value =
                $"No. on Roll at the beginning of Month: {totalStudents}  No. of School Days: {totalWorkingDays}   Average No. on Roll: {averageNoOnRoll}";
                worksheet.Range(rollSummaryStart, 1, rollSummaryStart, totalColumns - 8).Merge();

                // Calculate the number of students admitted during the month from table3
                
                // Add the calculated value for "Admitted during the month" to the summary
                worksheet.Cell(rollSummaryStart + 1, 1).Value =
                    $"Admitted during the month: {newJoinedCount}   Left: {LeftStudnetCount}   Average Attendance: {averageAttendance}   During Month: {monthName}";
                worksheet.Range(rollSummaryStart + 1, 1, rollSummaryStart + 1, totalColumns - 8).Merge();

                worksheet.Cell(rollSummaryStart + 2, 1).Value = $"No. on Roll at the end of: {totalEnrolled}     During Month: {monthName} ";

                worksheet.Range(rollSummaryStart + 2, 1, rollSummaryStart + 2, totalColumns - 8).Merge();

                // 7. Certification Box - aligned right
                int certifyStartCol = totalColumns - 7;
                var certifyRange = worksheet.Range(rollSummaryStart, certifyStartCol, rollSummaryStart + 2, totalColumns);
                certifyRange.Merge();
                certifyRange.Value = "Certified attendance marked is correct\nH. M. / Teacher";
                certifyRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                certifyRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                certifyRange.Style.Alignment.SetWrapText(true);
                certifyRange.Style.Font.SetBold();
                certifyRange.Style.Font.SetFontSize(8);

                // 8. Adjustments
                worksheet.Columns().AdjustToContents();
                worksheet.Rows().AdjustToContents();

                worksheet.RangeUsed().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.RangeUsed().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                // 9. Apply thick outside border around the entire document content
            
                var usedRange = worksheet.RangeUsed();
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorderColor = XLColor.Black;
                usedRange.Style.Border.OutsideBorderColor = XLColor.Black;


                workbook.SaveAs(filePath);
            }

            return filePath;
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
                    return "User name missing";
                }
                return ex.Message.ToString();
            }
          
        }

    
        public Task<List<ExamsModel>> GetExams(int? id)
        {
            var spName = ConstantSPnames.SP_GETEXAMS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ExamsModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ExamsModel>> InsertExams(ExamsModel roleModel)
        {
            var spName = ConstantSPnames.SP_INSERTEXAMS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ExamsModel>(spName, new
            {
                Name = roleModel.Name,
                IsActive = roleModel.IsActive,
                // RoleId=roleMaster.RoleId  ,
                CreatedBy = roleModel.CreatedBy,
                CreatedDate = roleModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<ExamsModel>> UpdateExams(ExamsModel roleModel)
        {
            var spName = ConstantSPnames.SP_UPDATEEXAMS;
            return Task.Factory.StartNew(() => _db.Connection.Query<ExamsModel>(spName, new
            {
                Name = roleModel.Name,
                IsActive = roleModel.IsActive,
                Id = roleModel.Id,
                ModifiedBy = roleModel.ModifiedBy,
                ModifiedDate = roleModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<string> DeleteExams(int id)
        {
            var spName = ConstantSPnames.SP_DELETEEXAMS;
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("Id", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                return Task.Factory.StartNew(() => "Success");                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => ex.Message);
            }
        }
        public Task<List<HousePointModel>> GetHousePointDetails()
        {
            var spName = ConstantSPnames.SP_GETHOUSEPOINT;
            return Task.Factory.StartNew(() => _db.Connection.Query<HousePointModel>(spName, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<HouseActivity>> GetHouseActivity(int? id)
        {
            var spName = ConstantSPnames.SP_GETHOUSEACTIVITY;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseActivity>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<HouseActivity>> InsertHouseActivity(HouseActivity roleModel)
        {
            var spName = ConstantSPnames.SP_INSERTHOUSEACTIVITY;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseActivity>(spName, new
            {
                ActivityName = roleModel.ActivityName,
                HouseId = roleModel.HouseId,
                Point = roleModel.Point,

                StudentList = roleModel.StudentList,
                CreatedBy = roleModel.CreatedBy,
                CreatedDate = roleModel.CreatedDate,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<HouseActivity>> UpdateHouseActivity(HouseActivity roleModel)
        {
            var spName = ConstantSPnames.SP_UPDATEHOUSEACTIVITY;
            return Task.Factory.StartNew(() => _db.Connection.Query<HouseActivity>(spName, new
            {
                ActivityName = roleModel.ActivityName,
                HouseId = roleModel.HouseId,
                StudentList = roleModel.StudentList,
                Point = roleModel.Point,
                Id = roleModel.Id,
                ModifiedBy = roleModel.ModifiedBy,
                ModifiedDate = roleModel.ModifiedDate
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<string> DeleteHouseActivity(int id)
        {
            var spName = ConstantSPnames.SP_DELETEHOUSEACTIVITY;
            try
            {
                using SqlConnection sqlconnection = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                sqlconnection.Open();
                SqlCommand command = new SqlCommand(spName, sqlconnection);
                command.CommandType = CommandType.StoredProcedure; command.Parameters.Add("Id", SqlDbType.Int).Value = id; command.ExecuteNonQuery();
                return Task.Factory.StartNew(() => "Success");                 //return Task.Factory.StartNew(() => _db.Connection.Query<Department>(spName, new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => ex.Message);
            }
        }
    
        public Task<List<UpcomingCompetition>> GetAllUpcomingCompetition(string role, int? id)
        {
            var spName = ConstantSPnames.SP_GETALLUPCOMINGCOMPETITION;
            return Task.Factory.StartNew(() => _db.Connection.Query<UpcomingCompetition>(spName,
                new {Role=role, Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<UpcomingCompetition>> InsertUpcomingCompetition(UpcomingCompetition model)
        {
            // fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;

            var spName = ConstantSPnames.SP_INSERTUPCOMINGCOMPETITION;
            return Task.Factory.StartNew(() => _db.Connection.Query<UpcomingCompetition>(spName, new
            {
                EventDate=model.EventDate,
                EventName=model.EventName,
                EventDay = model.EventDay,
                Grade=model.Grade,
                EventTiming =model.EventTiming,
                Eligibility= model.Eligibility,
                Guidelines= model.Guidelines,
                TimeLimit= model.TimeLimit,
                JudgingCriteria= model.JudgingCriteria,
                DressCode= model.DressCode,
                IsPollingRequired= model.IsPollingRequired,
                PollingEndDate= model.PollingEndDate,
                CreatedBy = model.CreatedBy
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        
        public Task<List<UpcomingCompetition>> UpdateInterestedCompetition(int studentId, int competitionId)
        {
            var spName = ConstantSPnames.SP_UPDATEINTERESTED;
            return Task.Factory.StartNew(() => _db.Connection.Query<UpcomingCompetition>(spName, new
            {
                @StudentId=studentId,
                @CompetitionId=competitionId
            }, commandType: CommandType.StoredProcedure).ToList());

        }
        public Task<List<UpcomingCompetition>> UpdateUpcomingCompetition(UpcomingCompetition model)
        {
            var spName = ConstantSPnames.SP_UPDATEUPCOMINGCOMPETITION;
            return Task.Factory.StartNew(() => _db.Connection.Query<UpcomingCompetition>(spName, new
            {
                Id=model.Id,
                EventDate = model.EventDate,
                EventName = model.EventName,
                EventDay = model.EventDay,
                Grade = model.Grade,
                EventTiming = model.EventTiming,
                Eligibility = model.Eligibility,
                Guidelines = model.Guidelines,
                TimeLimit = model.TimeLimit,
                JudgingCriteria = model.JudgingCriteria,
                DressCode = model.DressCode,
                IsPollingRequired = model.IsPollingRequired,
                PollingEndDate = model.PollingEndDate,
                ModifiedBy = model.ModifiedBy
            }, commandType: CommandType.StoredProcedure).ToList());

        }
        public Task<List<UpcomingCompetition>> DeleteUpcomingCompetition(int id)
        {
            var spName = ConstantSPnames.SP_DELETEUPCOMINGCOMPETITION;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<UpcomingCompetition>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        public Task<List<LeaveModel>> GetAllLeave(string role, int? id)
        {
            var spName = ConstantSPnames.SP_GETALLLEAVE;
            return Task.Factory.StartNew(() => _db.Connection.Query<LeaveModel>(spName,
                new { Role = role, id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<LeaveModel>> InsertLeave(LeaveModel model)
        {
            // fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;

            var spName = ConstantSPnames.SP_INSERTLEAVE;
            return Task.Factory.StartNew(() => _db.Connection.Query<LeaveModel>(spName, new
            {
                StudentId=model.StudentId,
                LeaveType =model.LeaveType,
                Reason =model.Reason,
                DateOfLeave =model.DateOfLeave,
                CreatedBy = model.CreatedBy
            }, commandType: CommandType.StoredProcedure).ToList());
        }

       
        public Task<List<LeaveModel>> UpdateLeave(LeaveModel model)
        {
            var spName = ConstantSPnames.SP_UPDATELEAVE;
            return Task.Factory.StartNew(() => _db.Connection.Query<LeaveModel>(spName, new
            {
                Id = model.Id,
                StudentId = model.StudentId,
                LeaveType = model.LeaveType,
                Reason = model.Reason,
                DateOfLeave = model.DateOfLeave,
                ModifiedBy = model.ModifiedBy
            }, commandType: CommandType.StoredProcedure).ToList());

        }
        public Task<List<LeaveModel>> DeleteLeave(int id)
        {
            var spName = ConstantSPnames.SP_DELETELEAVE;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<LeaveModel>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }


        public string GetInterestedStudentList(int competitionId)
        {
            try
            {
                string spName = ConstantSPnames.SP_INTERESTEDSTUDENTLIST;
                

                string connectionString = _appSettings.ConnectionInfo.TransactionDatabase;

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@CompetitionId", SqlDbType.Int).Value = competitionId;
                    //command.CommandTimeout = 100000;

                    using (var adapter = new SqlDataAdapter(command))
                    using (var ds = new DataSet())
                    {
                        adapter.Fill(ds);
                        DataTable studentTable = ds.Tables[0];
                        return GenerateExcelusingDatatable(studentTable, "Competition");
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error generating mark report");
                return ex.Message;
            }
        }
        private string GenerateExcelusingDatatable(DataTable studentTable, string activityName)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                string filePath = Path.Combine(folderPath, "Report.xlsx");

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Competition");
                    int colCount = 1, rowCount = 1;

                    string eventValue = studentTable.Rows[0]["EventName"].ToString();
                    if (studentTable.Columns.Contains("EventName"))
                    {
                        studentTable.Columns.Remove("EventName");
                    }

                    int colMaxWidth = studentTable.Columns.Count;

                    // === Title Row: School Name ===
                    var titleRange = ws.Range(rowCount, colCount, rowCount, colMaxWidth);
                    titleRange.Merge();
                    titleRange.Value = "SONA VALLIAPPA PUBLIC SCHOOL";
                    titleRange.Style.Font.Bold = true;
                    titleRange.Style.Font.FontSize = 14;
                    titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    titleRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    titleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    titleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    titleRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    rowCount++;

                    // === Subtitle Row: Event Name - STUDENT LIST ===
                    var subtitleRange = ws.Range(rowCount, colCount, rowCount, colMaxWidth);
                    subtitleRange.Merge();
                    subtitleRange.Value = $"{eventValue} - STUDENT LIST".ToUpper();
                    subtitleRange.Style.Font.Bold = true;
                    subtitleRange.Style.Font.FontSize = 12;
                    subtitleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    subtitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    subtitleRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    subtitleRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    subtitleRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    subtitleRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    rowCount++;

                    // === Column Headers ===
                    var headers = new List<string> {
                "S.No", "Adminssion No", "Name of Student", "Grade", "Section", "Father Mobile No", "Polling Status"
            };

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cell = ws.Cell(rowCount, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;

                        ws.Column(i + 1).AdjustToContents();
                    }
                    rowCount++;

                    // === Insert Student Data ===
                    ws.Cell(rowCount, 1).InsertData(studentTable);

                    // === Apply Style to Data Rows ===
                    var dataStartRow = rowCount;
                    var totalRows = studentTable.Rows.Count;
                    var totalCols = colMaxWidth;

                    for (int i = 0; i < totalRows; i++)
                    {
                        for (int j = 0; j < totalCols; j++)
                        {
                            var cell = ws.Cell(dataStartRow + i, j + 1);
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    var fullRange = ws.Range(1, 1, rowCount + totalRows - 1, colMaxWidth);
                    fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    // === Save File ===
                    if (File.Exists(filePath)) File.Delete(filePath);
                    wb.SaveAs(filePath);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetAllMarkReport(string section, string subjects, string test)
        {
            try
            {
                string spName = ConstantSPnames.SP_MARKTEMPLATE;
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                string filePath = Path.Combine(folderPath, "Report.xlsx");
                string[] subjectArr = subjects.Split(',');
                string[] secArr = section.Split('-');
                string connectionString = _appSettings.ConnectionInfo.TransactionDatabase;

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SectionId", SqlDbType.Int).Value = Convert.ToInt64(secArr[2]);
                    command.CommandTimeout = 100000;

                    using (var adapter = new SqlDataAdapter(command))
                    using (var ds = new DataSet())
                    {
                        adapter.Fill(ds);
                        DataTable studentTable = ds.Tables[0];
                        return GenerateExcelReport(studentTable, secArr, subjectArr, test, filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error generating mark report");
                return ex.Message;
            }
        }
        private string GenerateExcelReport(DataTable studentTable, string[] secArr, string[] subjectArr, string test, string filePath)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Mark Template");
                    int colCount = 1, rowCount = 1;
                    int colMaxWidth = subjectArr.Length + 3;
                    int colPart = colMaxWidth / 2;

                    // Headers
                    ws.Cell(rowCount++, colCount).Value = "SONA VALLIAPPA PUBLIC SCHOOL";
                    ws.Range(rowCount - 1, colCount, rowCount - 1, colMaxWidth).Merge().AddToNamed("Titles");

                    ws.Cell(rowCount++, colCount).Value = $"STUDENT MARK REPORT-{test.ToUpper()}";
                    ws.Range(rowCount - 1, colCount, rowCount - 1, colMaxWidth).Merge().AddToNamed("Titles");

                    ws.Cell(rowCount, colCount).Value = "GRADE: " + secArr[0].ToUpper();
                    ws.Range(rowCount, colCount, rowCount, colPart).Merge().AddToNamed("Titles");

                    ws.Cell(rowCount++, colPart + 1).Value = "SECTION: " + secArr[1].ToUpper();
                    ws.Range(rowCount - 1, colPart + 1, rowCount - 1, colMaxWidth).Merge().AddToNamed("Titles");

                    // Column headers
                    var headers = new List<string> { "SNo", "Reg.No", "Name of Student" };
                    headers.AddRange(subjectArr);
                    for (int i = 0; i < headers.Count; i++)
                    {
                        ws.Cell(rowCount, i + 1).Value = headers[i];
                        ws.Column(i + 1).AdjustToContents().AddToNamed("Titles");
                    }
                    rowCount++;

                    ws.Cell(rowCount++, 1).Value = "Date of Examination";
                    ws.Range(rowCount - 1, 1, rowCount - 1, 3).Merge().AddToNamed("Titles");

                    // Insert Data
                    ws.Cell(rowCount, 1).InsertData(studentTable).AddToNamed("Titles");

                    ApplyStyles(wb, ws, studentTable.Rows.Count, colMaxWidth);

                    if (File.Exists(filePath)) File.Delete(filePath);
                    wb.SaveAs(filePath);
                }
                return filePath;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error generating Excel report");
                return ex.Message;
            }
        }
        private void ApplyStyles(XLWorkbook wb, IXLWorksheet ws, int studentCount, int colMaxWidth)
        {
            var style = wb.Style;
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            style.Border.InsideBorder = XLBorderStyleValues.Thin;
            style.Font.FontSize = 10;

            ws.Range(ws.Cell(1, 1), ws.Cell(studentCount + 1, colMaxWidth)).Style = style;
        }
        public async Task<string> bulkuploadmark(string target,  string section)
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
            dtColumn.DataType = typeof(Int32); 
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
                            data += subject[i] + "-" + row.Cell(i + 4).Value + " ||  Date : " + dateofexam[i] + ",";
                        }
                        //Add rows to DataTable.
                        dtRow = dt.NewRow();
                        dtRow["Id"] = 1;
                        dtRow["StudentId"] = row.Cell(2).Value.ToString();
                        dtRow["StudentName"] = row.Cell(3).Value.ToString();
                        dtRow["Section"] = Convert.ToInt32(section.Split('-')[2]);
                        dtRow["TestType"] = testtype.ToLower();
                        dtRow["Data"] = data;
                        dtRow["PreviousMonthAttendance"] = "-";
                        dtRow["IsattendanceRequired"] = false;
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
                mark.Section = Convert.ToInt32(rdr["Section"]);
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
       
        public async Task<Announcement> InsertAnnouncementDetails(Announcement announcement)
        {
           
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
        public Task<List<StudentDropdownModel>> GetMappedStudentByName(string StudentName, int SectionId)
        {
            var spName = ConstantSPnames.SP_GETMAPPEDSTUDENTBYNAME;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentDropdownModel>(spName, new
            {
               
                StudentName = StudentName, // Fixed variable case mismatch
                SectionId = SectionId      // Fixed variable case mismatch
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
                        AttendanceRecords = new Dictionary<string, string>()
                    };

                    // Handle dynamic columns
                    for (int i =2; i < result.FieldCount; i++) // Start from 5 assuming the first 4 columns are fixed
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
                SectionId = assignmentModel.SectionId,
                SubjectId = assignmentModel.SubjectId,
                Title = assignmentModel.Title,
                FacultyId = assignmentModel.FacultyId,
                Description = assignmentModel.Description,
                DueDate = assignmentModel.DueDate,
                FileName = assignmentModel.FileName,
                FilePath = assignmentModel.FilePath,
                CreatedBy = assignmentModel.CreatedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AssignmentModel>> UpdateAssignmentDetails(AssignmentModel assignmentModel)
        {
            var spName = ConstantSPnames.SP_UPDATEASSIGNMENTDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName, new
            {
                Id = assignmentModel.Id,
                SectionId = assignmentModel.SectionId,
                SubjectId = assignmentModel.SubjectId,
                Title = assignmentModel.Title,
                FacultyId = assignmentModel.FacultyId,
                Description = assignmentModel.Description,
                DueDate = assignmentModel.DueDate,
                FileName = assignmentModel.FileName,
                FilePath = assignmentModel.FilePath,
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
        public Task<List<AssignmentModel>> GetAllAssignmentByStudentDetails(string role,int studentId)
        {
            var spName = ConstantSPnames.SP_GETALLASSIGNMENTBYSTUDENT;
            return Task.Factory.StartNew(() => _db.Connection.Query<AssignmentModel>(spName,
                new { role= role,studentId = studentId }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<TimetableModel>> GetAllTimetableDetails(int? id)
        {
            var spName = ConstantSPnames.SP_GETALLTIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName,
                new { Id = id }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> GetTimeTableBySectionIdDetails(int sectionId,string role)
        {
            var spName = ConstantSPnames.SP_GetTimeTableBySectionIdDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName,
                new { SectionId= sectionId,Role=role }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<BatchSubjectFacultyModel>> GetFacultyListBySectionIdDetails(int sectionId)
        {
            var spName = ConstantSPnames.SP_GetFacultyListBySectionIdDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<BatchSubjectFacultyModel>(spName,
                new { SectionId = sectionId }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> InsertTimetableDetails(TimetableModel timetableModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTTIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName, new
            {
                
                Day = timetableModel.Day,
                SectionId = timetableModel.SectionId,
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
                
                CreatedBy = timetableModel.CreatedBy

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<TimetableModel>> UpdateTimetableDetails(TimetableModel timetableModel)
        {
            var spName = ConstantSPnames.SP_UPDATETIMETABLEDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<TimetableModel>(spName, new
            {
                Id = timetableModel.Id,
                Day = timetableModel.Day,
                SectionId = timetableModel.SectionId,
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


        public Task<List<StudentFeedbackModel>> GetAllStudentFeedbackDetails(int? id,string role)
        {
            var spName = ConstantSPnames.SP_GETALLSTUDENTFEEDBACKDETAILS;
            return Task.Factory.StartNew(() => _db.Connection.Query<StudentFeedbackModel>(spName,
                new { Id = id, Role=role }, commandType: CommandType.StoredProcedure).ToList());
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
                FacultyId = contentLibModel.FacultyId,
                SectionId = contentLibModel.SectionId,
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
                SectionId = contentLibModel.SectionId,
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

      
        public  Task<List<Feedbacksubject>> getSubFacultyList(int studentId)
        {

            //List<Feedbacksubject> subjectDetails;
            string spToGetfb = "sp_GetSubFacultyFeedbackNew";
            return Task.Factory.StartNew(() => _db.Connection.Query<Feedbacksubject>(spToGetfb,
              new { @StudentId = studentId }, commandType: CommandType.StoredProcedure).ToList());

        }
        public async Task<Feedback> getFeedbackDetails(int studentId, int subjectId, int facultyId)
        {
            List<Feedback> fb;
            int a;
            string GetFeedback = "[sp_GetFeedbackFormNew]";
            //string GetInsertedFeedback = "[sp_GetInsertedFeedback]";

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

            await using (SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString()))
            {
                await con.OpenAsync(); // Open the connection asynchronously

                using SqlCommand cmd = new SqlCommand(selectSQL, con);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader GrReader = await cmd.ExecuteReaderAsync(); // Execute reader asynchronously
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
            return _questions;
        }

        public async Task<string> InsertFeedBackDetails(Feedback feedback)
        {
            string selectSQL = "[sp_InsertFeedback]";
            DateTime now = DateTime.Now;

            try
            {
                await using SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
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
                await using SqlConnection con = new SqlConnection(_appSettings.ConnectionInfo.TransactionDatabase.ToString());
                SqlCommand cmd = new SqlCommand(selectSQL, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                con.Open();
                cmd.ExecuteNonQuery();
                return ("Success");
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public Task<List<AcademicCalender>> GetAllAcademicCalender(string role)
        {
            var spName = ConstantSPnames.SP_GETACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName,
                new { Role= role }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> InsertAcademicCalender(AcademicCalender academicCalender)
        {
            
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName, new
            {


                AcademicActivities=academicCalender.AcademicActivities,
                StartDate=academicCalender.StartDate,
                EndDate =academicCalender.EndDate,
                CreatedBy = academicCalender.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> UpdateAcademicCalender(AcademicCalender academicCalender)
        {
            var spName = ConstantSPnames.SP_UPDATEACADEMICCALENDER;
            return Task.Factory.StartNew(() => _db.Connection.Query<AcademicCalender>(spName, new
            {
                Id = academicCalender.Id,
                AcademicActivities = academicCalender.AcademicActivities,
                StartDate = academicCalender.StartDate,
                EndDate = academicCalender.EndDate,
                ModifiedBy = academicCalender.ModifiedBy,
                ModifiedDate = academicCalender.ModifiedDate
            },
        commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<AcademicCalender>> DeleteAcademicCalender(int id)
        {
            var spName = ConstantSPnames.SP_DELETEACADEMICCALENDER;
            return Task.Factory.StartNew(() =>
             _db.Connection.Query<AcademicCalender>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                 .ToList());
        }
        public Task<List<InfoGaloreModel>> GetAllInfoGalore(string infoType, int? id)
        {
            var spName = ConstantSPnames.SP_GETINFOGALORE;
            return Task.Factory.StartNew(() => _db.Connection.Query<InfoGaloreModel>(spName,
                new {@InfoType=infoType, @Id=id}, commandType: CommandType.StoredProcedure).ToList());
        }
        public Task<List<InfoGaloreModel>> InsertInfoGalore(InfoGaloreModel infoGaloreModel)
        {
            //fdpModel.MakerDate = fdpModel.IsMakerCompleted == true ? DateTime.Now : DateTime.MinValue;
            var spName = ConstantSPnames.SP_INSERTINFOGALORE;
            return Task.Factory.StartNew(() => _db.Connection.Query<InfoGaloreModel>(spName, new
            {

                InfoType=infoGaloreModel.InfoType,
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


        public Task<List<HolidayCalendar>> GetHolidayCalendar(int? id)
        {
            var spName = ConstantSPnames.SP_GETHOLIDAYCALENDAR;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<HolidayCalendar>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

        
        


        public Task<List<HolidayCalendar>> InsertHolidayCalendar(HolidayCalendar holiday)
        {
            var spName = ConstantSPnames.SP_INSERTHOLIDAYCALENDAR;
            return Task.Factory.StartNew(() => _db.Connection.Query<HolidayCalendar>(spName, new
            {
                DateofHoliday = holiday.DateofHoliday,
                HolidayDetails = holiday.HolidayDetails,


                CreatedBy = holiday.CreatedBy,

            }, commandType: CommandType.StoredProcedure).ToList());
        }


        

        public Task<List<HolidayCalendar>> UpdateHolidayCalendar(HolidayCalendar holiday)
        {
            var spName = ConstantSPnames.SP_UPDATEHOLIDAYCALENDAR;
            return Task.Factory.StartNew(() => _db.Connection.Query<HolidayCalendar>(spName, new
            {
                Id = holiday.Id,
                DateofHoliday = holiday.DateofHoliday,
                HolidayDetails=holiday.HolidayDetails,
                ModifiedBy = holiday.ModifiedBy,
            }, commandType: CommandType.StoredProcedure).ToList());
        }

        public Task<List<HolidayCalendar>> DeleteHolidayCalendar(int id)
        {
            var spName = ConstantSPnames.SP_DELETEHOLIDAYCALENDAR;
            return Task.Factory.StartNew(() =>
                _db.Connection.Query<HolidayCalendar>(spName, new { Id = id }, commandType: CommandType.StoredProcedure)
                    .ToList());
        }

    }
}
