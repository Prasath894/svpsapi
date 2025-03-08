using ActivityManagementSystem.BLL.Common;
using ActivityManagementSystem.BLL.Interfaces;
using ActivityManagementSystem.DAL;
using ActivityManagementSystem.DAL.Interfaces;
using ActivityManagementSystem.DAL.Repositories;
using ActivityManagementSystem.Domain.AppSettings;
using ActivityManagementSystem.Domain.Models;
using ActivityManagementSystem.Domain.Models.Activity;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace ActivityManagementSystem.BLL.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IDatabase<ActivityRepository> _activityRepository;
        private readonly AppSettings _appSettings;
        public ActivityService(IDatabase<ActivityRepository> activityRepository, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _activityRepository = activityRepository;
        }

        public virtual async Task<List<ActivityModel>> GetAllActivityData(int Type, long? DepartmentId)
        {
            try
            {

                List<ActivityModel> result = await _activityRepository.Repository.GetAllActivityData(Type, DepartmentId);

                for (int i = 0; i < result.Count; i++)
                {
                    var files = result[i].FileNames;
                    if (files != null)
                    {
                        result[i].Files = files.Split('|').ToList();
                        result[i].Files.RemoveAt(result[i].Files.Count - 1);
                    }
                }
         

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<Token> LoginAsync(LoginUserDto user)
        {
            try
            {
                string userRole = user.Role.ToString();

                if (user.Role == "Teacher" || user.Role == "Admin" || user.Role =="Principal")
                {
                    var faculties = await _activityRepository.Repository.GetUserDetails(user.Username, user.Password,user.Role);
                    if (faculties == null || faculties.Count == 0)  // Fix: Count should be compared with 0
                    {
                        throw new Exception("Server unavailable.");
                    }

                    var faculty = faculties.FirstOrDefault(x =>
                        (x.UserName?.Equals(user.Username) ?? false) &&
                        (x.Password?.Equals(user.Password) ?? false)); // Fix: Ensure both conditions match for login

                    if (faculty != null)
                    {
                        return GetToken(faculty.FacultyName, userRole, faculty.Id);
                    }
                    else
                    {
                        throw new Exception("The username and password do not match.");
                    }
                }
                else if (user.Role.ToString() == "Parent")
                {
                    var students = await _activityRepository.Repository.GetStudentDetails(null);
                    if (students == null || students.Count == 0)  // Fix: Count should be compared with 0
                    {
                        throw new Exception("Server unavailable.");
                    }

                    var student = students.FirstOrDefault(x =>
                        (x.Father_MobileNumber?.Equals(user.MobileNo) ?? false) ||
                        (x.Mother_MobileNumber?.Equals(user.MobileNo) ?? false));

                    if (student != null)
                    {
                        string userName = student.Father_MobileNumber.Equals(user.MobileNo) ?
                            student.FatherName : student.MotherName;
                        return GetToken(userName, userRole, student.Id);
                    }
                    else
                    {
                        throw new Exception("The mobile number does not match.");
                    }
                }

                // Default case: if user.Role is not Faculty or Parent
                throw new Exception("Invalid role specified.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message); // Better exception handling
            }
        }

        public async Task<List<AttachmentModel>> GetAttachmentAsync(int id, string type)
        {
            // Step 1: Define the action method based on the file type
            string actionMethodName = string.Empty;
            if (type == "Assignment")
            {
                actionMethodName = $"Assignment\\Assignment-{id}";
            }
            if (type == "Announcement")
            {
                actionMethodName = $"Announcement\\Announcement-{id}";
            }
            else if (type == "Students")
            {
                actionMethodName = $"Students\\Students-{id}";
            }
            else if (type == "ContentLib")
            {
                actionMethodName = $"ContentLib\\ContentLib-{id}";
            }
            else if (type == "Event")
            {
                actionMethodName = $"Events\\Events-{id}";
            }
            else if (type == "PressReports")
            {
                actionMethodName = $"PressReports\\PressReports-{id}";
            }
            else if (type == "Faculty")
            {
                actionMethodName = $"Facultys\\Facultys-{id}";
            }
            else if (type == "InfoGalore")
            {
                actionMethodName = "InfoGalore";
            }

            // Step 2: Define the source and destination folders
            string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), actionMethodName);
            string destination = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", type, $"{type}-{id}");

            // Step 3: Check if the source folder exists; if not, return an empty list
            if (!Directory.Exists(outputFolder))
            {
                return new List<AttachmentModel>(); // Return an empty list if folder does not exist
            }

            // Ensure the folder exists by deleting it first if it's already there
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            Directory.CreateDirectory(destination);

            // Step 4: Get all files from the source folder
            string[] files = Directory.GetFiles(outputFolder);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destination, fileName);
                File.Copy(file, destFile, true); // Overwrites if file exists
            }

            List<AttachmentModel> extractedFiles = new List<AttachmentModel>();

            // Step 5: Process extracted files
            foreach (var filePath in Directory.GetFiles(destination))
            {
                var attachment = new AttachmentModel
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath
                };

                if (FileIsAnImageChecker.IsImageFile(filePath))
                {
                    attachment.BlobData = await File.ReadAllBytesAsync(filePath);
                }

                extractedFiles.Add(attachment);
            }

            return extractedFiles;
        }

        private Token GetToken(string userName, string userRole, int userId)
        {
            var jwtService = new JwtService(_appSettings.JWTSettings.SecretKey ?? string.Empty,
                    _appSettings.JWTSettings.Issuer ?? string.Empty,_appSettings.JWTSettings.Audience ?? string.Empty);

            var claims = new List<Claim>()
                {
                    new (ClaimTypes.Name, userName),
                    new (ClaimTypes.Role, userRole),
                    new Claim("Role", userRole.ToString()),
                    new Claim("UserId", userId.ToString())
                };

            var expires = DateTime.UtcNow.AddHours(2);
            var token = jwtService.GenerateToken(claims, expires);
            return new Token()
            {
                AccessToken = token,
                ExpiresAt = expires,
                UserRole = userRole,
                Username = userName,
                UserId = userId
            };
        }
        public virtual async Task<List<ActivityModel>> GetActivityData(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetActivityData(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ActivityModel>> InsertActivityData(ActivityModel activityData)
        {
            try
            {
                return await _activityRepository.Repository.InsertActivityData(activityData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ActivityModel>> UpdateActivityData(ActivityModel activityData)
        {
            try
            {
                return await _activityRepository.Repository.UpdateActivityData(activityData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ActivityModel>> DeleteActivityData(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteActivityData(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public virtual async Task<List<RoleModel>> GetRole(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetRole(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<RoleActivity>> GetRoleActivity(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetRoleActivity(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<RoleModel>> InsertRole(RoleModel rolemaster)
        {
            try
            {
                return await _activityRepository.Repository.InsertRole(rolemaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<RoleModel>> UpdateRole(RoleModel rolemaster)
        {
            try
            {
                return await _activityRepository.Repository.UpdateRole(rolemaster);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<RoleActivity>> UpdateRoleActivity(RoleActivity roleactivity)
        {
            try
            {
                return await _activityRepository.Repository.UpdateRoleActivity(roleactivity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<RoleActivity>> InsertRoleActivity(RoleActivity roleactivity)
        {
            try
            {
                return await _activityRepository.Repository.InsertRoleActivity(roleactivity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteRole(int id)
        {
            try
            {
                return _activityRepository.Repository.DeleteRole(id);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentModel>> GetStudentDetails(int? Id)
        {
            try
            {
                return await _activityRepository.Repository.GetStudentDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public virtual async Task<List<Department>> GetAllDepartment(int? Id)
        //{
        //    try
        //    {
        //        return await _activityRepository.Repository.GetAllDepartment(Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public virtual async Task<List<StudentModel>> InsertStudentDetails(StudentModel studentDetails)
        {
            try
            {
                return await _activityRepository.Repository.InsertStudentDetails(studentDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<HouseModel>> InsertHouseDetails(HouseModel house)
        {
            try
            {
                return await _activityRepository.Repository.InsertHouseDetails(house);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentModel>> UpdateStudentDetails(StudentModel studentDetails)
        {
            try
            {
                return await _activityRepository.Repository.UpdateStudentDetails(studentDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<HouseModel>> UpdateHouseDetails(HouseModel house)
        {
            try
            {
                return await _activityRepository.Repository.UpdateHouseDetails(house);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentModel>> DeleteStudentDetails(int Id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteStudentDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Task<List<HouseModel>> DeleteHouseDetails(int Id)
        {
            try
            {
                return _activityRepository.Repository.DeleteHouseDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<FacultyModel>> GetFacultyDetails(int? Id)
        {
            try
            {
                return await _activityRepository.Repository.GetFacultyDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string InsertFacultyDetails(FacultyModel facultyDetails)
        {
            try
            {
                return _activityRepository.Repository.InsertFacultyDetails(facultyDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string UpdateFacultyDetails(FacultyModel facultyDetails)
        {
            try
            {
                return _activityRepository.Repository.UpdateFacultyDetails(facultyDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<FacultyModel>> DeleteFacultyDetails(int Id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteFacultyDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<string> UpdateActivityFilepathdata(string target, int id, string files)
        {
            try
            {
                return await _activityRepository.Repository.UpdateActivityFilepathdata(target, id, files);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<string> UpdateFilepathdata(string target, int id, string files, string TypeofUser)
        {
            try
            {
                return await _activityRepository.Repository.UpdateFilepathdata(target, id, files, TypeofUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public virtual async Task<List<FacultyDropdown>> GetFacultyByName(string facultyName)
        {
            try
            {
                return await _activityRepository.Repository.GetFacultyByName(facultyName);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


     
        public virtual async Task<List<HouseModel>> GetAllHouse(int? Id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllHouse(Id);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentDropdown>> GetStudentByName(string studentName)
        {
            try
            {
                return await _activityRepository.Repository.GetStudentByName(studentName);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public virtual async Task<List<DepartmentModel>> GetAllDepartment(int? id)
        //{
        //    try
        //    {
        //        return await _activityRepository.Repository.GetAllDepartment(id);
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public virtual async Task<List<SubjectModel>> GetAllSubject(int? Id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllSubject(Id);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SubjectModel>> UpdateSubjectDetails(SubjectModel subject)
        {
            try
            {
                return await _activityRepository.Repository.UpdateSubjectDetails(subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteSubjectDetails(int Id)
        {
            try
            {
                return _activityRepository.Repository.DeleteSubjectDetails(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SubjectModel>> InsertSubjectDetails(SubjectModel subject)
        {
            try
            {
                return await _activityRepository.Repository.InsertSubjectDetails(subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SectionModel>> GetAllSectiones(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllSectiones(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SectionModel>> InsertSectionDetails(SectionModel section)
        {
            try
            {
                return await _activityRepository.Repository.InsertSectionDetails(section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SectionModel>> UpdateSectionDetails(SectionModel section)
        {
            try
            {
                return await _activityRepository.Repository.UpdateSectionDetails(section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SectionModel>> DeleteSectionDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteSectionDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<BatchStudMappingModel>> GetAllSectionStudMappings(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllSectionStudMappings(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<int> InsertSectionStudMappings(List<BatchStudMappingModel> data)
        {
            try
            {
                return await _activityRepository.Repository.InsertSectionStudMappings(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<int> UpdateSectionStudMapping(List<BatchStudMappingModel> model)
        {
            try
            {
                return await _activityRepository.Repository.UpdateSectionStudMapping(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<int> DeleteSectionStudMapping(int[] ids, int batchId)
        {
            try
            {
                return await _activityRepository.Repository.DeleteSectionStudMapping(ids, batchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> bulkuploadstudent(DataTable target)
        {
            try
            {
                return await _activityRepository.Repository.bulkuploadstudent(target);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     
        
        public async Task <string> bulkuploadsubject(DataTable target)
        {
            try
            {
                return await _activityRepository.Repository.bulkuploadfaculty(target);
                return await _activityRepository.Repository.bulkuploadsubject(target);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> bulkuploadfaculty(DataTable target)
        {
            try
            {
                return await _activityRepository.Repository.bulkuploadfaculty(target);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<string> bulkuploadmark(string target,  string section)
        {
            try
            {
                return await _activityRepository.Repository.bulkuploadmark(target,  section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AttendanceModel>> GetAllAttendance(DateTime? AttendanceDate, int sectionId, string Hoursday)
        {
            try
            {
                return await _activityRepository.Repository.GetAllAttendance(AttendanceDate,  sectionId, Hoursday);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string InsertAttendance(List<AttendanceModel> attendance)
        {
            try
            {
                return _activityRepository.Repository.InsertAttendance(attendance);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AttendanceModel>> UpdateAttendance(AttendanceModel attendance)
        {
            try
            {
                return await _activityRepository.Repository.UpdateAttendance(attendance);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteAttendance(List<AttendanceModel> attendance)
        {
            try
            {
                return _activityRepository.Repository.DeleteAttendance(attendance);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ActivityResponse>> GetAllActivityDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ActivityResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ActivityResponse>(x.Data)).ToList();

                for (int i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (int k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //data[i].FromDate = Convert.ToString(data[i].FromDate).ToString("DD-MM-YYYY");
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                    // string date = 
                }

                var viewModel = new ActivityResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetFilteredResult(data, selectedColumn).ToList();

                                // result.AddRange(filterResult);
                            }
                        }
                    }
                }

                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<ActivityResponse> GetFilteredResult(IEnumerable<ActivityResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<ActivityResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {

                case "Abstract":
                    response = activityResponse.Where(x => x.Abstract == filterColumn.Value);

                    return response;

                case "Topic":
                    response = activityResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;

                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);
                    return response;

                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);
                    return response;

                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "Prize":
                    response = activityResponse.Where(x => x.Prize == filterColumn.Value);

                    return response;
                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "Department":
                    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "OffLineOnLineOrBlended":
                    response = activityResponse.Where(x => x.OffLineOnLineOrBlended == filterColumn.Value);

                    return response;
                //case "StudentID":
                //    response = activityResponse.Where(x => x.StudentID == filterColumn.Value);

                //    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }

        #region Prasath

        public virtual async Task<List<ProjectModelResponse>> GetProjectModelForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ProjectModelResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ProjectModelResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //data[i].FromDate = Convert.ToString(data[i].FromDate).ToString("DD-MM-YYYY");
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                    // string date = 
                }
                var viewModel = new ProjectModelResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetProjectModelFilteredResult(data, selectedColumn).ToList();

                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<ProjectModelResponse> GetProjectModelFilteredResult(IEnumerable<ProjectModelResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<ProjectModelResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {
                case "Abstract":
                    response = activityResponse.Where(x => x.Abstract == filterColumn.Value);

                    return response;

                case "Topic":
                    response = activityResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;

                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);
                    return response;

                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);
                    return response;

                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);
                    return response;
                case "ProjectTitle":
                    response = activityResponse.Where(x => x.ProjectTitle == filterColumn.Value);
                    return response;
                case "MentorName":
                    response = activityResponse.Where(x => x.MentorName == filterColumn.Value);
                    return response;
                case "ProjectDescription":
                    response = activityResponse.Where(x => x.ProjectDescription == filterColumn.Value);
                    return response;
                case "Mode":
                    response = activityResponse.Where(x => x.Mode == filterColumn.Value);
                    return response;
                case "ProjectCostApprox":
                    response = activityResponse.Where(x => x.ProjectCostApprox == Convert.ToDecimal(filterColumn.Value));
                    return response;
                case "FutureScope":
                    response = activityResponse.Where(x => x.FutureScope == filterColumn.Value);
                    return response;
                case "Type":
                    response = activityResponse.Where(x => x.Type == filterColumn.Value);
                    return response;
                case "DesignType":
                    response = activityResponse.Where(x => x.DesignType == filterColumn.Value);
                    return response;
                case "Prize":
                    response = activityResponse.Where(x => x.Prize == filterColumn.Value);
                    return response;
                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }


        public virtual async Task<List<ImplantTrainingResponse>> GetImplantTrainingReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ImplantTrainingResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ImplantTrainingResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new ImplantTrainingResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetImplantTrainingModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        private IEnumerable<ImplantTrainingResponse> GetImplantTrainingModelFilteredResult(IEnumerable<ImplantTrainingResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<ImplantTrainingResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "OrganisationName":
                    response = activityResponse.Where(x => x.OrganisationName == filterColumn.Value);
                    return response;

                case "OrganisationAddress":
                    response = activityResponse.Where(x => x.OrganisationAddress == filterColumn.Value);
                    return response;

                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);
                    return response;

                case "Remarks":
                    response = activityResponse.Where(x => x.Remarks == filterColumn.Value);

                    return response;
                case "Department":
                    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }


        public virtual async Task<List<SportsandGamesResponse>> GetSportsGamesReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<SportsandGamesResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<SportsandGamesResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new SportsandGamesResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetSportsGamesModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<SportsandGamesResponse> GetSportsGamesModelFilteredResult(IEnumerable<SportsandGamesResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<SportsandGamesResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {



                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "Prize":
                    response = activityResponse.Where(x => x.Prize == filterColumn.Value);
                    return response;

                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;
                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "TeamName":
                    response = activityResponse.Where(x => x.TeamName == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;


                default:
                    response = activityResponse;

                    return response;
            }
        }


        public virtual async Task<List<NSSResponse>> GetNSSReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<NSSResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<NSSResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new NSSResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetNSSModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<NSSResponse> GetNSSModelFilteredResult(IEnumerable<NSSResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<NSSResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {



                case "TeamName":
                    response = activityResponse.Where(x => x.TeamName == filterColumn.Value);

                    return response;
                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);
                    return response;

                case "Prize":
                    response = activityResponse.Where(x => x.Prize == filterColumn.Value);
                    return response;

                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;


                default:
                    response = activityResponse;

                    return response;
            }
        }

        public virtual async Task<List<FacultyDevelopmentResponse>> GetFacultyDevelopmentReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<FacultyDevelopmentResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<FacultyDevelopmentResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new FacultyDevelopmentResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetFacultyDevelopementModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<FacultyDevelopmentResponse> GetFacultyDevelopementModelFilteredResult(IEnumerable<FacultyDevelopmentResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<FacultyDevelopmentResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {

                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "FDPSponsoredBy":
                    response = activityResponse.Where(x => x.FDPSponsoredBy == filterColumn.Value);
                    return response;

                case "SanctionedAmount":
                    response = activityResponse.Where(x => x.SanctionedAmount == filterColumn.Value);
                    return response;
                case "TitleOfTheProgram":
                    response = activityResponse.Where(x => x.TitleOfTheProgram == filterColumn.Value);
                    return response;
                case "TrainingAttendedBy":
                    response = activityResponse.Where(x => x.TrainingAttendedBy == filterColumn.Value);
                    return response;

                case "ParticipantsType":
                    response = activityResponse.Where(x => x.ParticipantsType == filterColumn.Value);
                    return response;

                case "NParticipants":
                    response = activityResponse.Where(x => x.NParticipants == filterColumn.Value);

                    return response;
                case "CoOrdinator":
                    response = activityResponse.Where(x => x.CoOrdinator == filterColumn.Value);

                    return response;
                case "CoCoordinator":
                    response = activityResponse.Where(x => x.CoCoordinator == filterColumn.Value);

                    return response;

                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;

                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "OnlineOrOffline":
                    response = activityResponse.Where(x => x.OnlineOrOffline == filterColumn.Value);

                    return response;



                default:
                    response = activityResponse;

                    return response;
            }
        }


        public virtual async Task<List<WomenDevelopementResposnse>> GetWomenDevelopmentReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<WomenDevelopementResposnse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<WomenDevelopementResposnse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new WomenDevelopementResposnse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetWomenDevelopementModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        private IEnumerable<WomenDevelopementResposnse> GetWomenDevelopementModelFilteredResult(IEnumerable<WomenDevelopementResposnse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<WomenDevelopementResposnse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {



                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);
                    return response;
                case "Title":
                    response = activityResponse.Where(x => x.Title == filterColumn.Value);
                    return response;
                case "ParticipantsFrom":
                    response = activityResponse.Where(x => x.ParticipantsFrom == filterColumn.Value);
                    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;
                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "OnlineOrOffline":
                    response = activityResponse.Where(x => x.OnlineOrOffline == filterColumn.Value);

                    return response;

                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }


        public virtual async Task<List<SemesterResultResponse>> GetSemesterResultReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<SemesterResultResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<SemesterResultResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                }
                var viewModel = new SemesterResultResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetSemesterResultModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        private IEnumerable<SemesterResultResponse> GetSemesterResultModelFilteredResult(IEnumerable<SemesterResultResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<SemesterResultResponse> response = activityResponse;
            switch (filterColumn.Name)
            {



                case "Department":
                    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                    return response;
                case "NStudentAppeared":
                    response = activityResponse.Where(x => x.NStudentAppeared == filterColumn.Value);

                    return response;
                case "NStudentPassed":
                    response = activityResponse.Where(x => x.NStudentPassed == filterColumn.Value);

                    return response;
                case "NBoysAppeared":
                    response = activityResponse.Where(x => x.NBoysAppeared == filterColumn.Value);
                    return response;

                case "NGirlsAppeared":
                    response = activityResponse.Where(x => x.NGirlsAppeared == filterColumn.Value);
                    return response;

                case "NBoysPassed":
                    response = activityResponse.Where(x => x.NBoysPassed == filterColumn.Value);
                    return response;

                case "NGirlsPassed":
                    response = activityResponse.Where(x => x.NGirlsPassed == filterColumn.Value);

                    return response;
                case "SemMonth":
                    response = activityResponse.Where(x => x.SemMonth == filterColumn.Value);

                    return response;
                case "Sem":

                    response = activityResponse.Where(x => x.Sem == filterColumn.Value);

                    return response;
                case "Year":

                    response = activityResponse.Where(x => x.Year == filterColumn.Value);

                    return response;
                case "PassPercentage":
                    response = activityResponse.Where(x => x.PassPercentage == filterColumn.Value);

                    return response;
                case "Remarks":
                    response = activityResponse.Where(x => x.Remarks == filterColumn.Value);

                    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }



        public virtual async Task<List<SymposiumExpoResponse>> GetSymposiumResultReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<SymposiumExpoResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<SymposiumExpoResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new SymposiumExpoResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetSymposiumExpoModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<SymposiumExpoResponse> GetSymposiumExpoModelFilteredResult(IEnumerable<SymposiumExpoResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<SymposiumExpoResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "Abstract":
                    response = activityResponse.Where(x => x.Abstract == filterColumn.Value);

                    return response;
                case "Event":
                    response = activityResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = activityResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "Venue":
                    response = activityResponse.Where(x => x.Venue == filterColumn.Value);
                    return response;
                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = activityResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "Impact":
                    response = activityResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = activityResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "StudentOrFaculty":
                    response = activityResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;
                case "InternalOrExternal":
                    response = activityResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "ThemeOrSubTheme":
                    response = activityResponse.Where(x => x.ThemeOrSubTheme == filterColumn.Value);

                    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }



        public virtual async Task<List<PlacementResponse>> GetPlacementResultReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<PlacementResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<PlacementResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];

                }
                var viewModel = new PlacementResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetPlacementModelFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<PlacementResponse> GetPlacementModelFilteredResult(IEnumerable<PlacementResponse> activityResponse, FilterColumn filterColumn)
        {
            IEnumerable<PlacementResponse> response = activityResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {

                case "FromDate":
                    response = activityResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "CompanyName":
                    response = activityResponse.Where(x => x.CompanyName == filterColumn.Value);

                    return response;
                case "CompanyAddress":
                    response = activityResponse.Where(x => x.CompanyAddress == filterColumn.Value);

                    return response;
                case "CompanyType":
                    response = activityResponse.Where(x => x.CompanyType == filterColumn.Value);
                    return response;

                case "Department":
                    response = activityResponse.Where(x => x.Department == filterColumn.Value);
                    return response;

                case "NStudentAttended":
                    response = activityResponse.Where(x => x.NStudentAttended == filterColumn.Value);
                    return response;

                case "NStudentAttendedBoys":
                    response = activityResponse.Where(x => x.NStudentAttendedBoys == filterColumn.Value);

                    return response;
                case "NStudentAttendedGirls":
                    response = activityResponse.Where(x => x.NStudentAttendedGirls == filterColumn.Value);

                    return response;
                case "NStudentPlaced":
                    response = activityResponse.Where(x => x.NStudentPlaced == filterColumn.Value);

                    return response;
                case "NStudentPlacedBoys":
                    response = activityResponse.Where(x => x.NStudentPlacedBoys == filterColumn.Value);

                    return response;
                case "NStudentPlacedGirls":
                    response = activityResponse.Where(x => x.NStudentPlacedGirls == filterColumn.Value);

                    return response;

                case "OffcampusOrOncampus":
                    response = activityResponse.Where(x => x.OffcampusOrOncampus == filterColumn.Value);

                    return response;

                case "SalaryPerAnnum":
                    response = activityResponse.Where(x => x.SalaryPerAnnum == filterColumn.Value);

                    return response;

                default:
                    response = activityResponse;

                    return response;
            }
        }

        


        
        #endregion

        #region Nandhini



        public virtual async Task<List<MiscellaneousResponse>> GetAllMiscellaneousDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<MiscellaneousResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<MiscellaneousResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new MiscellaneousResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetMiscellaneousFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<MiscellaneousResponse> GetMiscellaneousFilteredResult(IEnumerable<MiscellaneousResponse> miscellaneousResponse, FilterColumn filterColumn)
        {
            IEnumerable<MiscellaneousResponse> response = miscellaneousResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "Event":
                    response = miscellaneousResponse.Where(x => x.Event == filterColumn.Value);
                    return response;
                case "ActivityName":
                    response = miscellaneousResponse.Where(x => x.ActivityName == filterColumn.Value);
                    return response;

                case "Remarks":
                    response = miscellaneousResponse.Where(x => x.Remarks == filterColumn.Value);
                    return response;

                case "Venue":
                    response = miscellaneousResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "Impact":
                    response = miscellaneousResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "FromDate":
                    response = miscellaneousResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = miscellaneousResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "Department":
                    response = miscellaneousResponse.Where(x => x.Department == filterColumn.Value);

                    return response;
                case "StudentOrFaculty":
                    response = miscellaneousResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = miscellaneousResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "OnlineOrOffline":
                    response = miscellaneousResponse.Where(x => x.OnlineOrOffline == filterColumn.Value);

                    return response;
                default:
                    response = miscellaneousResponse;

                    return response;
            }
        }



        public virtual async Task<List<IndustrialVisitResponse>> GetAllIndustrialVisitDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<IndustrialVisitResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<IndustrialVisitResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitVisit = data[i].DateToVisit.Split('/');
                    data[i].DateToVisit = dateSplitVisit[1] + '/' + dateSplitVisit[0] + '/' + dateSplitVisit[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new IndustrialVisitResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetIndustrialVisitFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private IEnumerable<IndustrialVisitResponse> GetIndustrialVisitFilteredResult(IEnumerable<IndustrialVisitResponse> industrialVisitResponse, FilterColumn filterColumn)
        {
            IEnumerable<IndustrialVisitResponse> response = industrialVisitResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {
                case "Visit":
                    response = industrialVisitResponse.Where(x => x.Visit == filterColumn.Value);
                    return response;

                case "Remarks":
                    response = industrialVisitResponse.Where(x => x.Remarks == filterColumn.Value);
                    return response;

                case "OrganisationName":
                    response = industrialVisitResponse.Where(x => x.OrganisationName == filterColumn.Value);

                    return response;
                case "Impact":
                    response = industrialVisitResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "FromDate":
                    response = industrialVisitResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = industrialVisitResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "StudentOrFaculty":
                    response = industrialVisitResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "OrganisationAddress":
                    response = industrialVisitResponse.Where(x => x.OrganisationAddress == filterColumn.Value);

                    return response;
                case "DateToVisit":

                    response = industrialVisitResponse.Where(x =>

                   Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateToVisit.Split('/')[2],
                                                                      x.DateToVisit.Split('/')[1],
                                                                      x.DateToVisit.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "NParticipants":
                    response = industrialVisitResponse.Where(x => x.NParticipants == Convert.ToInt16(filterColumn.Value));

                    return response;
                case "Department":
                    response = industrialVisitResponse.Where(x => x.Department == filterColumn.Value);

                    return response;
                default:
                    response = industrialVisitResponse;

                    return response;
            }
        }


        public virtual async Task<List<NccResponse>> GetAllNccDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<NccResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<NccResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new NccResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetNccFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<NccResponse> GetNccFilteredResult(IEnumerable<NccResponse> nccResponse, FilterColumn filterColumn)
        {
            IEnumerable<NccResponse> response = nccResponse;

            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {

                case "Event":
                    response = nccResponse.Where(x => x.Event == filterColumn.Value);
                    return response;
                case "TeamName":
                    response = nccResponse.Where(x => x.TeamName == filterColumn.Value);
                    return response;

                case "Prize":
                    response = nccResponse.Where(x => x.Prize == filterColumn.Value);
                    return response;

                case "Venue":
                    response = nccResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "OrganisedBy":
                    response = nccResponse.Where(x => x.OrganisedBy == filterColumn.Value);

                    return response;
                case "FromDate":
                    response = nccResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = nccResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "StudentOrFaculty":
                    response = nccResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = nccResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = nccResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                default:
                    response = nccResponse;

                    return response;
            }
        }


        public virtual async Task<List<ConsultantResponse>> GetAllConsultantDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ConsultantResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ConsultantResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].ProjectGivenDate.Split('/');
                    data[i].ProjectGivenDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ProjectCompletionDate.Split('/');
                    data[i].ProjectCompletionDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new ConsultantResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetConsultantFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<ConsultantResponse> GetConsultantFilteredResult(IEnumerable<ConsultantResponse> consultantResponse, FilterColumn filterColumn)
        {
            IEnumerable<ConsultantResponse> response = consultantResponse;

            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "ProjectName":
                    response = consultantResponse.Where(x => x.ProjectName == filterColumn.Value);
                    return response;
                case "ProblemStatement":
                    response = consultantResponse.Where(x => x.ProblemStatement == filterColumn.Value);
                    return response;

                case "MethodologyOpt":
                    response = consultantResponse.Where(x => x.MethodologyOpt == filterColumn.Value);
                    return response;

                case "ConsultancyFees":
                    response = consultantResponse.Where(x => x.ConsultancyFees == filterColumn.Value);

                    return response;
                case "Organisation":
                    response = consultantResponse.Where(x => x.Organisation == filterColumn.Value);

                    return response;
                case "ProjectGivenDate":
                    response = consultantResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ProjectGivenDate.Split('/')[2],
                                                                       x.ProjectGivenDate.Split('/')[1],
                                                                       x.ProjectGivenDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ProjectCompletionDate":
                    response = consultantResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ProjectCompletionDate.Split('/')[2],
                                                                       x.ProjectCompletionDate.Split('/')[1],
                                                                       x.ProjectCompletionDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;


                case "StandardsIfAny":
                    response = consultantResponse.Where(x => x.StandardsIfAny == filterColumn.Value);

                    return response;
                case "SupportingStaff":
                    response = consultantResponse.Where(x => x.SupportingStaff == filterColumn.Value);

                    return response;

                case "InternalOrExternal":
                    response = consultantResponse.Where(x => x.InternalOrExternal == filterColumn.Value);

                    return response;
                case "OnlineOrOffline":
                    response = consultantResponse.Where(x => x.OnlineOrOffline == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = consultantResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                default:
                    response = consultantResponse;

                    return response;
            }
        }


        public virtual async Task<List<PatentResponse>> GetAllPatentDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<PatentResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<PatentResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                }
                var viewModel = new PatentResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetPatentFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private IEnumerable<PatentResponse> GetPatentFilteredResult(IEnumerable<PatentResponse> patentResponse, FilterColumn filterColumn)
        {
            IEnumerable<PatentResponse> response = patentResponse;
            switch (filterColumn.Name)
            {

                case "PatentTitle":
                    response = patentResponse.Where(x => x.PatentTitle == filterColumn.Value);
                    return response;
                case "PatentRegistrationDetails":
                    response = patentResponse.Where(x => x.PatentRegistrationDetails == filterColumn.Value);
                    return response;

                case "PatentType":
                    response = patentResponse.Where(x => x.PatentType == filterColumn.Value);
                    return response;

                case "Abstract":
                    response = patentResponse.Where(x => x.Abstract == filterColumn.Value);
                    return response;
                case "Status":
                    response = patentResponse.Where(x => x.Status == filterColumn.Value);
                    return response;
                case "Mentor":
                    response = patentResponse.Where(x => x.Mentor == filterColumn.Value);

                    return response;
                case "StudentOrFaculty":
                    response = patentResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;
                case "NationalOrInternational":
                    response = patentResponse.Where(x => x.NationalOrInternational == filterColumn.Value);

                    return response;
                //case "Department":
                //    response = patentResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;

                default:
                    response = patentResponse;

                    return response;
            }
        }


        public virtual async Task<List<ExtensionServicesResponse>> GetAllExtensionServicesDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ExtensionServicesResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ExtensionServicesResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].ToDate.Split('/');
                    data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new ExtensionServicesResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetExtensionFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<ExtensionServicesResponse> GetExtensionFilteredResult(IEnumerable<ExtensionServicesResponse> extensionServicesResponse, FilterColumn filterColumn)
        {
            IEnumerable<ExtensionServicesResponse> response = extensionServicesResponse;

            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {

                case "Topic":
                    response = extensionServicesResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;
                case "FromDate":
                    response = extensionServicesResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "ToDate":
                    response = extensionServicesResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.ToDate.Split('/')[2],
                                                                        x.ToDate.Split('/')[1],
                                                                        x.ToDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "Participants":
                    response = extensionServicesResponse.Where(x => x.Participants == filterColumn.Value);
                    return response;
                case "CourseFees":
                    response = extensionServicesResponse.Where(x => x.CourseFees == Convert.ToDecimal(filterColumn.Value));
                    return response;
                case "NParticipants":
                    response = extensionServicesResponse.Where(x => x.NParticipants == Convert.ToDouble(filterColumn.Value));

                    return response;
                case "NParticipantsBoys":
                    response = extensionServicesResponse.Where(x => x.NParticipantsBoys == Convert.ToDouble(filterColumn.Value));

                    return response;
                case "NParticipantsGirls":
                    response = extensionServicesResponse.Where(x => x.NParticipantsGirls == Convert.ToDouble(filterColumn.Value));

                    return response;
                case "OrganisedBy":
                    response = extensionServicesResponse.Where(x => x.OrganisedBy == filterColumn.Value);
                    return response;
                case "TotalRevenueGenerated":
                    response = extensionServicesResponse.Where(x => x.TotalRevenueGenerated == filterColumn.Value);
                    return response;
                case "Department":
                    response = extensionServicesResponse.Where(x => x.Department == filterColumn.Value);

                    return response;
                case "ResourcePersonDetails ":
                    response = extensionServicesResponse.Where(x => x.ResourcePersonDetails == filterColumn.Value);

                    return response;
                case "Impact":
                    response = extensionServicesResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "Remarks":
                    response = extensionServicesResponse.Where(x => x.Remarks == filterColumn.Value);

                    return response;

                default:
                    response = extensionServicesResponse;

                    return response;
            }
        }

        public virtual async Task<List<GrantsResponse>> GetAllGrantsDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<GrantsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<GrantsResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                }
                var viewModel = new GrantsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetGrantsFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<GrantsResponse> GetGrantsFilteredResult(IEnumerable<GrantsResponse> grantsResponse, FilterColumn filterColumn)
        {
            IEnumerable<GrantsResponse> response = grantsResponse;
            switch (filterColumn.Name)
            {

                case "SanctionedBy":
                    response = grantsResponse.Where(x => x.SanctionedBy == filterColumn.Value);
                    return response;
                case "SanctionedYear":
                    response = grantsResponse.Where(x => x.SanctionedYear == filterColumn.Value);
                    return response;

                case "GrantGivenBy":
                    response = grantsResponse.Where(x => x.GrantGivenBy == filterColumn.Value);
                    return response;

                case "ProjectName":
                    response = grantsResponse.Where(x => x.ProjectName == filterColumn.Value);
                    return response;

                case "SanctionedGrantAmount":
                    response = grantsResponse.Where(x => x.SanctionedGrantAmount == filterColumn.Value);
                    return response;
                case "RecurringAmount":
                    response = grantsResponse.Where(x => x.RecurringAmount == filterColumn.Value);
                    return response;
                case "NonRecurringAmount":
                    response = grantsResponse.Where(x => x.NonRecurringAmount == filterColumn.Value);
                    return response;
                case "SanctionedOrderNo":
                    response = grantsResponse.Where(x => x.SanctionedOrderNo == filterColumn.Value);
                    return response;


                case "Department":
                    response = grantsResponse.Where(x => x.Department == filterColumn.Value);

                    return response;

                default:
                    response = grantsResponse;

                    return response;
            }
        }
        #endregion

        public virtual async Task<List<ActivityGuestlecturesResponse>> GetGuestlecturesForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<ActivityGuestlecturesResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<ActivityGuestlecturesResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].FromDate.Split('/');
                    data[i].FromDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];

                }
                var viewModel = new ActivityGuestlecturesResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetGuestlecturesFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private IEnumerable<ActivityGuestlecturesResponse> GetGuestlecturesFilteredResult(IEnumerable<ActivityGuestlecturesResponse> guestlecturesResponse, FilterColumn filterColumn)
        {
            IEnumerable<ActivityGuestlecturesResponse> response = guestlecturesResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {
                case "ResourcePerson":
                    response = guestlecturesResponse.Where(x => x.ResourcePerson == filterColumn.Value);

                    return response;

                case "Topic":
                    response = guestlecturesResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;

                case "OrganisedBy":
                    response = guestlecturesResponse.Where(x => x.OrganisedBy == filterColumn.Value);
                    return response;

                case "ParticipantsFrom":
                    response = guestlecturesResponse.Where(x => x.ParticipantsFrom == filterColumn.Value);

                    return response;
                case "NParticipants":
                    response = guestlecturesResponse.Where(x => x.NParticipants == filterColumn.Value);

                    return response;

                case "FromDate":
                    response = guestlecturesResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.FromDate.Split('/')[2],
                                                                        x.FromDate.Split('/')[1],
                                                                        x.FromDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "Time":
                    response = guestlecturesResponse.Where(x => x.Time == filterColumn.Value);

                    return response;
                case "ResourcePersonDetails":
                    response = guestlecturesResponse.Where(x => x.ResourcePersonDetails == filterColumn.Value);

                    return response;

                case "ExternalParticipants":
                    response = guestlecturesResponse.Where(x => x.ExternalParticipants == filterColumn.Value);

                    return response;
                case "NParticipantsGirls":
                    response = guestlecturesResponse.Where(x => x.NParticipantsGirls == filterColumn.Value);

                    return response;
                case "NParticipantsBoys":
                    response = guestlecturesResponse.Where(x => x.NParticipantsBoys == filterColumn.Value);

                    return response;

                case "Department":
                    response = guestlecturesResponse.Where(x => x.Department == filterColumn.Value);

                    return response;

                case "OnlineOrOffline":
                    response = guestlecturesResponse.Where(x => x.OnlineOrOffline == filterColumn.Value);
                    return response;
                default:
                    response = guestlecturesResponse;

                    return response;
            }
        }


        public virtual async Task<List<JournalResponse>> GetJournalForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<JournalResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<JournalResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    if (data[i].JournalIssueDate != "")
                    {
                        string[] dateSplitFrom = data[i].JournalIssueDate.Split('/');
                        data[i].JournalIssueDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    }
                    if (data[i].BookIssueDate != "")
                    {
                        string[] dateSplitTo = data[i].BookIssueDate.Split('/');
                        data[i].BookIssueDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                    }

                }
                var viewModel = new JournalResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetJournalFilteredResult(data, selectedColumn).ToList();
                                //result.AddRange(filterResult);
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private IEnumerable<JournalResponse> GetJournalFilteredResult(IEnumerable<JournalResponse> journalResponse, FilterColumn filterColumn)
        {
            IEnumerable<JournalResponse> response = journalResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                filterColumn.Value.Split('/')[1],
                filterColumn.Value.Split('/')[0]);

            }
            switch (filterColumn.Name)
            {
                case "JournalIssueDate":
                    response = journalResponse.Where(x => x.JournalIssueDate != "").Where(x =>
                   Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.JournalIssueDate.Split('/')[2],
                                                                      x.JournalIssueDate.Split('/')[1],
                                                                      x.JournalIssueDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "JournalPaperTitle":
                    response = journalResponse.Where(x => x.JournalPaperTitle == filterColumn.Value);
                    return response;


                case "JournalAbstract":
                    response = journalResponse.Where(x => x.JournalAbstract == filterColumn.Value);
                    return response;

                case "JournalName":
                    response = journalResponse.Where(x => x.JournalName == filterColumn.Value);

                    return response;
                case "JournalPublisher":
                    response = journalResponse.Where(x => x.JournalPublisher == filterColumn.Value);

                    return response;
                case "TotalJournalPages":
                    response = journalResponse.Where(x => x.TotalJournalPages == Convert.ToDouble(filterColumn.Value));

                    return response;
                case "JournalType":
                    response = journalResponse.Where(x => x.JournalType == filterColumn.Value);

                    return response;
                case "BookIssueDate":

                    response = journalResponse.Where(x => x.BookIssueDate != "").Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.BookIssueDate.Split('/')[2],
                                                                                          x.BookIssueDate.Split('/')[1],
                                                                                          x.BookIssueDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "BookName":
                    response = journalResponse.Where(x => x.BookName == filterColumn.Value);

                    return response;
                case "BookPublishingOn":
                    response = journalResponse.Where(x => x.BookPublishingOn == filterColumn.Value);

                    return response;
                case "BookAuthorName":
                    response = journalResponse.Where(x => x.BookAuthorName == filterColumn.Value);

                    return response;
                case "BookCoAuthor":
                    response = journalResponse.Where(x => x.BookCoAuthor == filterColumn.Value);

                    return response;
                case "BookPublisherName":
                    response = journalResponse.Where(x => x.BookPublisherName == filterColumn.Value);

                    return response;
                case "BookVolumeNo":
                    response = journalResponse.Where(x => x.BookVolumeNo == filterColumn.Value);

                    return response;
                case "BookType":
                    response = journalResponse.Where(x => x.BookType == filterColumn.Value);

                    return response;
                case "BookPublicationNumber":
                    response = journalResponse.Where(x => x.BookPublicationNumber == filterColumn.Value);

                    return response;
                case "BookRemarks":
                    response = journalResponse.Where(x => x.BookRemarks == filterColumn.Value);

                    return response;

                case "Department":
                    response = journalResponse.Where(x => x.Department == filterColumn.Value);

                    return response;

                case "StudentOrFaculty":
                    response = journalResponse.Where(x => x.StudentOrFaculty == filterColumn.Value);

                    return response;
                default:
                    response = journalResponse;

                    return response;
            }
        }

        public virtual async Task<List<AwardsResponse>> GetAllAwardsDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<AwardsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<AwardsResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //string[] dateSplitFrom = data[i].DateofAnnouncement.Split('/');
                    //data[i].DateofAnnouncement = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].DateOfAwardCeremony.Split('/');
                    data[i].DateOfAwardCeremony = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new AwardsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetAwardsFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<AwardsResponse> GetAwardsFilteredResult(IEnumerable<AwardsResponse> awardsResponse, FilterColumn filterColumn)
        {
            IEnumerable<AwardsResponse> response = awardsResponse;
            if (filterColumn.Name.ToLower().Contains("dateofaward"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "NameOfTheAwards":
                    response = awardsResponse.Where(x => x.NameOfTheAwards == filterColumn.Value);
                    return response;
                case "NameOfTheInstitution":
                    response = awardsResponse.Where(x => x.NameOfTheInstitution == filterColumn.Value);
                    return response;
                case "NameOfThePrincipal":
                    response = awardsResponse.Where(x => x.NameOfThePrincipal == filterColumn.Value);
                    return response;

                case "NameOfOrganisers":
                    response = awardsResponse.Where(x => x.NameOfOrganisers == filterColumn.Value);
                    return response;

                case "Venue":
                    response = awardsResponse.Where(x => x.Venue == filterColumn.Value);

                    return response;
                case "DateOfAnnouncement":
                    response = awardsResponse.Where(x => x.DateOfAnnouncement == filterColumn.Value);

                    //Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateofAnnouncement.Split('/')[2],
                    //                                                    x.DateofAnnouncement.Split('/')[1],
                    //                                                    x.DateofAnnouncement.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "DateOfAwardCeremony":
                    response = awardsResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateOfAwardCeremony.Split('/')[2],
                                                                        x.DateOfAwardCeremony.Split('/')[1],
                                                                        x.DateOfAwardCeremony.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;

                //case "Department":
                //    response = awardsResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "StudentOrFaculty":
                    response = awardsResponse.Where(x => x.DescriptionOfAwards == filterColumn.Value);

                    return response;
                case "AwardBestowedTo":
                    response = awardsResponse.Where(x => x.AwardBestowedTo == filterColumn.Value);

                    return response;

                case "Event":
                    response = awardsResponse.Where(x => x.Event == filterColumn.Value);

                    return response;
                case "Year":
                    response = awardsResponse.Where(x => x.Year == filterColumn.Value);

                    return response;
                default:
                    response = awardsResponse;

                    return response;
            }
        }

        public virtual async Task<List<MOUsResponse>> GetAllMOUsDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<MOUsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<MOUsResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //string[] dateSplitFrom = data[i].DateofAnnouncement.Split('/');
                    //data[i].DateofAnnouncement = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplit = data[i].MOUDate.Split('/');
                    data[i].MOUDate = dateSplit[1] + '/' + dateSplit[0] + '/' + dateSplit[2];
                    string[] dateSplitTo = data[i].MOUStartDate.Split('/');
                    data[i].MOUStartDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                    string[] dateSplitFrom = data[i].MOUEndDate.Split('/');
                    data[i].MOUEndDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];

                }
                var viewModel = new MOUsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetMOUsFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private IEnumerable<MOUsResponse> GetMOUsFilteredResult(IEnumerable<MOUsResponse> mOUsResponse, FilterColumn filterColumn)
        {
            IEnumerable<MOUsResponse> response = mOUsResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "NameOfTheIndustryOrOrganization":
                    response = mOUsResponse.Where(x => x.NameOfTheIndustryOrOrganization == filterColumn.Value);
                    return response;
                case "InstitutionOrProgramme":
                    response = mOUsResponse.Where(x => x.InstitutionOrProgramme == filterColumn.Value);
                    return response;
                case "NatureOfServicesByTheInstitutionOrOrganization":
                    response = mOUsResponse.Where(x => x.NatureOfServicesByTheInstitutionOrOrganization == filterColumn.Value);
                    return response;


                case "MOUStartDate":
                    response = mOUsResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.MOUStartDate.Split('/')[2],
                                                                        x.MOUStartDate.Split('/')[1],
                                                                        x.MOUStartDate.Split('/')[0])) >= Convert.ToDateTime(filterColumn.Value));
                    return response;

                case "MOUEndDate":
                    response = mOUsResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.MOUEndDate.Split('/')[2],
                                                                        x.MOUEndDate.Split('/')[1],
                                                                        x.MOUEndDate.Split('/')[0])) <= Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "MOUDate":
                    response = mOUsResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.MOUDate.Split('/')[2],
                                                                        x.MOUDate.Split('/')[1],
                                                                        x.MOUDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;


                default:
                    response = mOUsResponse;
                    return response;
            }
        }
        public virtual async Task<List<AdvertisementsResponse>> GetAllAdDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<AdvertisementsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<AdvertisementsResponse>(x.Data)).ToList();

                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //string[] dateSplitFrom = data[i].DateofAnnouncement.Split('/');
                    //data[i].DateofAnnouncement = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].DateOfPublished.Split('/');
                    data[i].DateOfPublished = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new AdvertisementsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetAdFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private IEnumerable<AdvertisementsResponse> GetAdFilteredResult(IEnumerable<AdvertisementsResponse> advertisementsResponse, FilterColumn filterColumn)
        {
            IEnumerable<AdvertisementsResponse> response = advertisementsResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "NameOfNewspaper":
                    response = advertisementsResponse.Where(x => x.NameOfNewspaper == filterColumn.Value);
                    return response;
                case "NewspaperEdition":
                    response = advertisementsResponse.Where(x => x.NewspaperEdition == filterColumn.Value);
                    return response;
                case "NewspaperPage":
                    response = advertisementsResponse.Where(x => x.NewspaperPage == filterColumn.Value);
                    return response;

                case "AdvertisementType":
                    response = advertisementsResponse.Where(x => x.AdvertisementType == filterColumn.Value);
                    return response;

                case "DescriptionOfAdmissions":
                    response = advertisementsResponse.Where(x => x.DescriptionOfAdmissions == filterColumn.Value);

                    return response;
                case "DescriptionOfEvent":
                    response = advertisementsResponse.Where(x => x.DescriptionOfEvent == filterColumn.Value);

                    //Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateofAnnouncement.Split('/')[2],
                    //                                                    x.DateofAnnouncement.Split('/')[1],
                    //                                                    x.DateofAnnouncement.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;
                case "DateOfPublished":
                    response = advertisementsResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateOfPublished.Split('/')[2],
                                                                        x.DateOfPublished.Split('/')[1],
                                                                        x.DateOfPublished.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;

                //case "Department":
                //    response = awardsResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "DescriptionofWanted":
                    response = advertisementsResponse.Where(x => x.DescriptionofWanted == filterColumn.Value);

                    return response;
                case "InstitutionProgrammesOrCIICP":
                    response = advertisementsResponse.Where(x => x.InstitutionProgrammesOrCIICP == filterColumn.Value);

                    return response;

                default:
                    response = advertisementsResponse;

                    return response;
            }
        }
        public virtual async Task<List<PressResponse>> GetAllPressDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<PressResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<PressResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    //string[] dateSplitFrom = data[i].DateofAnnouncement.Split('/');
                    //data[i].DateofAnnouncement = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    string[] dateSplitTo = data[i].DateOfPublished.Split('/');
                    data[i].DateOfPublished = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new PressResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetPressFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<PressResponse> GetPressFilteredResult(IEnumerable<PressResponse> pressResponse, FilterColumn filterColumn)
        {
            IEnumerable<PressResponse> response = pressResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "DescriptionOfFunction":
                    response = pressResponse.Where(x => x.DescriptionOfFunction == filterColumn.Value);
                    return response;
                case "DescriptionOfEvent":
                    response = pressResponse.Where(x => x.DescriptionOfEvent == filterColumn.Value);
                    return response;
                case "DescriptionOfAward":
                    response = pressResponse.Where(x => x.DescriptionOfAward == filterColumn.Value);
                    return response;

                case "DescriptionOfOthers":
                    response = pressResponse.Where(x => x.DescriptionOfOthers == filterColumn.Value);
                    return response;

                case "NameOfNewspaper":
                    response = pressResponse.Where(x => x.NameOfNewspaper == filterColumn.Value);

                    return response;

                case "DateOfPublished":
                    response = pressResponse.Where(x =>
                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.DateOfPublished.Split('/')[2],
                                                                        x.DateOfPublished.Split('/')[1],
                                                                        x.DateOfPublished.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;

                //case "Department":
                //    response = awardsResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "NewspaperEdition":
                    response = pressResponse.Where(x => x.NewspaperEdition == filterColumn.Value);

                    return response;
                case "NewspaperPage":
                    response = pressResponse.Where(x => x.NewspaperPage == filterColumn.Value);

                    return response;

                case "PressReportsType":
                    response = pressResponse.Where(x => x.PressReportsType == filterColumn.Value);

                    return response;
                case "InstitutionProgrammesOrCIICP":
                    response = pressResponse.Where(x => x.InstitutionProgrammesOrCIICP == filterColumn.Value);

                    return response;
                default:
                    response = pressResponse;

                    return response;
            }
        }
        public virtual async Task<List<EventsResponse>> GetAllEventsDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<EventsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<EventsResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].EventDate.Split('/');
                    data[i].EventDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    //string[] dateSplitTo = data[i].ToDate.Split('/');
                    //data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new EventsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetEventsFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<EventsResponse> GetEventsFilteredResult(IEnumerable<EventsResponse> eventsResponse, FilterColumn filterColumn)
        {
            IEnumerable<EventsResponse> response = eventsResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "Event":
                    response = eventsResponse.Where(x => x.EventName == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest":
                    response = eventsResponse.Where(x => x.NameOfTheChiefGuest == filterColumn.Value);
                    return response;

                case "Topic":
                    response = eventsResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;

                case "TypeOfEvent":
                    response = eventsResponse.Where(x => x.TypeOfEvent == filterColumn.Value);

                    return response;
                case "Impact":
                    response = eventsResponse.Where(x => x.Impact == filterColumn.Value);

                    return response;
                case "EventDate":
                    response = eventsResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.EventDate.Split('/')[2],
                                                                        x.EventDate.Split('/')[1],
                                                                        x.EventDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;


                //case "Department":
                //    response = eventsResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "nParticipants":
                    response = eventsResponse.Where(x => x.nParticipants == filterColumn.Value);

                    return response;
                default:
                    response = eventsResponse;

                    return response;
            }
        }
        public virtual async Task<List<UpcomingEventsResponse>> GetAllUpcomingEventsDataForReport(ActivityFilterModel activityFilterModel)
        {
            try
            {
                var result = new List<UpcomingEventsResponse>();

                var reportData = await GetAllActivityData(activityFilterModel.Type, activityFilterModel.DepartmentId);

                var data = reportData.Select(x => JsonConvert.DeserializeObject<UpcomingEventsResponse>(x.Data)).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> lstk = new List<string>();
                    if (reportData[i].Files != null)
                    {
                        for (var k = 0; k < reportData[i].Files.Count; k++)
                        {
                            string ext = System.IO.Path.GetExtension(reportData[i].Files[k].Trim()).Substring(1);
                            if (ext == "jpg" || ext == "bmp" || ext == "png" || ext == "jpeg")
                            {
                                string path = reportData[i].FilePath.ToString() + "\\" + reportData[i].Files[k].Trim();
                                byte[] b = System.IO.File.ReadAllBytes(path);
                                string val = Convert.ToBase64String(b);
                                lstk.Add(val);

                            }
                        }
                    }

                    data[i].FileBlob = lstk;
                    string[] dateSplitFrom = data[i].EventDate.Split('/');
                    data[i].EventDate = dateSplitFrom[1] + '/' + dateSplitFrom[0] + '/' + dateSplitFrom[2];
                    //string[] dateSplitTo = data[i].ToDate.Split('/');
                    //data[i].ToDate = dateSplitTo[1] + '/' + dateSplitTo[0] + '/' + dateSplitTo[2];
                }
                var viewModel = new UpcomingEventsResponse();

                var columns = viewModel.GetType().GetProperties();
                if (activityFilterModel.FilterColumns != null)
                {
                    if (activityFilterModel.FilterColumns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            var selectedColumn = activityFilterModel.FilterColumns.SingleOrDefault(x => x.Name == column.Name);

                            if (selectedColumn != null)
                            {
                                data = GetUpcomingEventsFilteredResult(data, selectedColumn).ToList();
                            }
                        }
                    }
                }
                result.AddRange(data);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<UpcomingEventsResponse> GetUpcomingEventsFilteredResult(IEnumerable<UpcomingEventsResponse> upcomingeventsResponse, FilterColumn filterColumn)
        {
            IEnumerable<UpcomingEventsResponse> response = upcomingeventsResponse;
            if (filterColumn.Name.ToLower().Contains("date"))
            {
                filterColumn.Value = String.Format("{0}/{1}/{2}", filterColumn.Value.Split('/')[2],
                    filterColumn.Value.Split('/')[1],
                    filterColumn.Value.Split('/')[0]);
            }
            switch (filterColumn.Name)
            {


                case "Event":
                    response = upcomingeventsResponse.Where(x => x.EventName == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest":
                    response = upcomingeventsResponse.Where(x => x.NameOfTheChiefGuest == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest2":
                    response = upcomingeventsResponse.Where(x => x.NameOfTheChiefGuest2 == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest3":
                    response = upcomingeventsResponse.Where(x => x.NameOfTheChiefGuest3 == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest4":
                    response = upcomingeventsResponse.Where(x => x.NameOfTheChiefGuest4 == filterColumn.Value);
                    return response;
                case "NameOfTheChiefGuest5":
                    response = upcomingeventsResponse.Where(x => x.NameOfTheChiefGuest5 == filterColumn.Value);
                    return response;
                case "Topic":
                    response = upcomingeventsResponse.Where(x => x.Topic == filterColumn.Value);
                    return response;

                case "TypeOfEvent":
                    response = upcomingeventsResponse.Where(x => x.TypeOfEvent == filterColumn.Value);

                    return response;

                case "EventDate":
                    response = upcomingeventsResponse.Where(x =>

                    Convert.ToDateTime(string.Format("{0}/{1}/{2}", x.EventDate.Split('/')[2],
                                                                        x.EventDate.Split('/')[1],
                                                                        x.EventDate.Split('/')[0])) == Convert.ToDateTime(filterColumn.Value));
                    return response;


                //case "Department":
                //    response = eventsResponse.Where(x => x.Department == filterColumn.Value);

                //    return response;
                case "nParticipants":
                    response = upcomingeventsResponse.Where(x => x.nParticipants == filterColumn.Value);

                    return response;
                default:
                    response = upcomingeventsResponse;

                    return response;
            }
        }


        //public virtual async Task<List<BatchStudentSubjectList>> GetBatchWiseStudentList()
        //{
        //    try
        //    {
        //        return await _activityRepository.Repository.GetBatchWiseStudentList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public virtual async Task<List<string>> GetSection(SectionModel sectionModel)
        //{
        //    try
        //    {
        //        return await _activityRepository.Repository.GetSection(sectionModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public virtual async Task<List<Batchdetails>> GetBatchList(BatchListModel batchListModel)
        {
            try
            {
                return await _activityRepository.Repository.GetBatchList(batchListModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<int> PasswordReset(string userName, string password)
        {
            try
            {
                return await _activityRepository.Repository.PasswordReset(userName, password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<BatchSubjectFacultyModel>> GetAllBatchSubMappings(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllBatchSubMapping(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BatchSubjectFacultyModel>> InsertBatchSubMappings(BatchSubjectFacultyModel data)

        {
            try
            {
                return await _activityRepository.Repository.InsertBatchSubMappings(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BatchSubjectFacultyModel>> UpdateBatchSubMapping(BatchSubjectFacultyModel data)

        {
            try
            {
                return await _activityRepository.Repository.UpdateBatchSubMapping(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual  string DeleteBatchSubMapping(int id)
        {
            try
            {
                return  _activityRepository.Repository.DeleteBatchSubMapping(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public virtual string generateAttendancereport()
        //{
        //    return  _activityRepository.Repository.generateAttendancereport();
        //}
       
        public virtual async Task<List<SubjectAttendanceModel>> Getsubjectsforattendance(string batch, string Department, string Sem, string Year, string Section)
        {
            try
            {
                return await _activityRepository.Repository.Getsubjectsforattendance(batch, Department, Sem, Year, Section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(MemoryStream memory, string path)> DownloadData(string filepath)
        {
            var Path = filepath;
            var memorys = new MemoryStream();
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                await stream.CopyToAsync(memorys);
            }
            return (memory: memorys, path: Path);
        }
        public virtual string generateMonthlyAttendancereport(string Sem, string Year, int Department, DateTime AttendanceDate, string Section)
        {
            return _activityRepository.Repository.generateMonthlyAttendancereport(Sem, Year, Department, AttendanceDate, Section);

        }
        public virtual string generateSubjectwiseMonthlyreport(string Sem, string Year, int Department, DateTime AttendanceDate, string Section, string SubjectCode)
        {
            return _activityRepository.Repository.generateSubjectwiseMonthlyreport(Sem, Year, Department, AttendanceDate, Section, SubjectCode);

        }
        public virtual string generateAttendancedynamicreport(string Sem, string Year, int Department, string Section)
        {
            return _activityRepository.Repository.generateAttendancedynamicreport(Sem, Year, Department, Section);

        }

        public virtual string generateAttendancesubjectwisereport(string SubjectCode, string Sem, string Year, string DepartmentId)
        {
            return _activityRepository.Repository.generateAttendancesubjectwisereport(SubjectCode, Sem, Year, DepartmentId);

        }
        public virtual string generateAttendanceSemwisereport(string Sem, string Year, int Department, string Section, string AcademicFrom, string AcademicTo)
        {
            return _activityRepository.Repository.generateAttendanceSemwisereport(Sem, Year, Department, Section, AcademicFrom, AcademicTo);
        }
        public virtual async Task<List<FacultyModel>> GetVerifyPassword(string UserName, string Password)
        {
            try
            {
                return await _activityRepository.Repository.GetVerifyPassword(UserName, Password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string UpdateVerifyPassword(string UserName, string NewPassword, string OldPassword, long FacultyId)
        {
            try
            {
                return _activityRepository.Repository.UpdateVerifyPassword(UserName, NewPassword, OldPassword, FacultyId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<IndentModel>> GetAllIndentDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllIndentDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<IndentModel>> InsertIndentDetails(IndentModel indentModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertIndentDetails(indentModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<IndentModel>> UpdateIndentDetails(IndentModel indentModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateIndentDetails(indentModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<IndentModel>> UpdateQuatationStatusDetails(QuatationModel quatationModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateQuatationStatusDetails(quatationModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<IndentModel>> DeleteIndentDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteIndentDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ExamsModel>> GetExams(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetExams(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ExamsModel>> InsertExams(ExamsModel roleModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertExams(roleModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ExamsModel>> UpdateExams(ExamsModel roleModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateExams(roleModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> DeleteExams(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteExams(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HousePointModel>> GetHousePointDetails()
        {
            try
            {
                return await _activityRepository.Repository.GetHousePointDetails();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HouseActivity>> GetHouseActivity(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetHouseActivity(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HouseActivity>> InsertHouseActivity(HouseActivity roleModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertHouseActivity(roleModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HouseActivity>> UpdateHouseActivity(HouseActivity roleModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateHouseActivity(roleModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> DeleteHouseActivity(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteHouseActivity(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string SearchAndReplaceIndentForm(int id)
        {
            try
            {
                return _activityRepository.Repository.SearchAndReplaceIndentForm(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string SearchAndReplaceFdpForm(int id)
        {
            try
            {
                return _activityRepository.Repository.SearchAndReplaceFdpForm(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UpcomingCompetition>> GetAllUpcomingCompetition(string role, int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllUpcomingCompetition(role,id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UpcomingCompetition>> InsertUpcomingCompetition(UpcomingCompetition model)
        {
            try
            {
                return await _activityRepository.Repository.InsertUpcomingCompetition(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UpcomingCompetition>> UpdateUpcomingCompetition(UpcomingCompetition model)
        {
            try
            {
                return await _activityRepository.Repository.UpdateUpcomingCompetition(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UpcomingCompetition>> DeleteUpcomingCompetition(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteUpcomingCompetition(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string SearchAndReplaceQuatationForm(int id, int flag)
        {
            try
            {
                return _activityRepository.Repository.SearchAndReplaceQuatationForm(id, flag);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryModel>> GetInventory(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetInventory(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryModel>> InsertInventory(InventoryModel inventoryModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertInventory(inventoryModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryModel>> UpdateInventory(InventoryModel inventoryModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateInventory(inventoryModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteInventory(int id)
        {
            try
            {
                return _activityRepository.Repository.DeleteInventory(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<InventorySpecModel>> GetInventorySpec(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetInventorySpec(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventorySpecModel>> InsertInventorySpec(InventorySpecModel inventorySpecModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertInventorySpec(inventorySpecModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventorySpecModel>> UpdateInventorySpec(InventorySpecModel inventorySpecModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateInventorySpec(inventorySpecModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteInventorySpec(int id)
        {
            try
            {
                return _activityRepository.Repository.DeleteInventorySpec(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<StockInventoryModel>> GetStockInventory(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetStockInventory(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StockInventoryModel>> InsertStockInventory(StockInventoryModel stockInventoryModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertStockInventory(stockInventoryModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StockInventoryModel>> UpdateStockInventory(StockInventoryModel stockInventoryModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateStockInventory(stockInventoryModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StockInventoryModel>> DeleteStockInventory(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteStockInventory(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<InventoryIssuedMappingModel>> GetAllInventoryIssuedDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllInventoryIssuedDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryIssuedMappingModel>> InsertInventoryIssuedDetails(InventoryIssuedMappingModel inventoryIssuedMappingModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertInventoryIssuedDetails(inventoryIssuedMappingModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryIssuedMappingModel>> UpdateInventoryIssuedDetails(InventoryIssuedMappingModel inventoryIssuedMappingModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateInventoryIssuedDetails(inventoryIssuedMappingModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InventoryIssuedMappingModel>> DeleteInventoryIssuedDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteInventoryIssuedDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DownloadStockReport(DateTime StockDate, string? Store)
        {
            return _activityRepository.Repository.DownloadStockReport(StockDate, Store);

        }
        
        public virtual async Task<List<HOADetailsModel>> GetAllHOADetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllHOADetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HOADetailsModel>> InsertHOADetails(HOADetailsModel hoaDetailsModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertHOADetails(hoaDetailsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<HOADetailsModel>> UpdateHOADetails(HOADetailsModel hoaDetailsModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateHOADetails(hoaDetailsModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DeleteHOADetails(int id)
        {
            try
            {
                return _activityRepository.Repository.DeleteHOADetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<PurchasedOrderModel>> GetPurchasedOrder(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetPurchasedOrder(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<PurchasedOrderModel>> InsertPurchasedOrder(PurchasedOrderModel purchasedOrderModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertPurchasedOrder(purchasedOrderModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<PurchasedOrderModel>> UpdatePurchasedOrder(PurchasedOrderModel purchasedOrderModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdatePurchasedOrder(purchasedOrderModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<PurchasedOrderModel>> DeletePurchasedOrder(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeletePurchasedOrder(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<BudgetLineModel>> GetBudgetLine(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetBudgetLine(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BudgetLineModel>> InsertBudgetLine(BudgetLineModel budgetLineModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertBudgetLine(budgetLineModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BudgetLineModel>> UpdateBudgetLine(BudgetLineModel budgetLineModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateBudgetLine(budgetLineModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> DeleteBudgetLine(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteBudgetLine(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BudgetHeadModel>> GetBudgetHead(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetBudgetHead(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BudgetReallocateModel>> ReallocateBudgetDetails(BudgetReallocateModel budgetReallocateModel)
        {
            try
            {
                return await _activityRepository.Repository.ReallocateBudgetDetails(budgetReallocateModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DownloadCumulativeReport(string? Store)
        {
            return _activityRepository.Repository.DownloadCumulativeReport(Store);
        }
        public virtual async Task<List<OdpModel>> GetAllOdpDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllOdpDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<OdpModel>> InsertOdpDetails(OdpModel oDPModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertOdpDetails(oDPModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<OdpModel>> UpdateOdpDetails(OdpModel oDPModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateOdpDetails(oDPModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<OdpModel>> DeleteOdpDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteOdpDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BpeModel>> GetAllBpeDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllBpeDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BpeModel>> InsertBpeDetails(BpeModel bpeModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertBpeDetails(bpeModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BpeModel>> UpdateBpeDetails(BpeModel bpeModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateBpeDetails(bpeModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BpeModel>> DeleteBpeDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteBpeDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string SearchAndReplaceBpeForm(int id)
        {
            try
            {
                return _activityRepository.Repository.SearchAndReplaceBpeForm(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string SearchAndReplaceOdpForm(int id)
        {
            try
            {
                return _activityRepository.Repository.SearchAndReplaceOdpForm(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<SubjectAttendanceModel>> Getsubjectsformarks(string Department, string Sem, string Year, string Section)
        {
            try
            {
                return await _activityRepository.Repository.Getsubjectsformarks(Department, Sem, Year, Section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string GetAllMarkReport(string Section, string subjects, string test)
        {
            return _activityRepository.Repository.GetAllMarkReport(Section, subjects, test);
        }
        public virtual List<StudentMark> GetStudentMark()
        {
            try
            {
                return _activityRepository.Repository.GetStudentMark();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<StudentMark>> InsertStudentMark(StudentMark studentmark)
        {
            try
            {
                return await _activityRepository.Repository.InsertStudentMark(studentmark);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<FacultySubjectMapping>> GetAllFacultySubMappings(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllFacultySubMappings(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string InsertFacultySubMappings(FacultySubjectMapping facultySubjectMapping)
        {
            try
            {
                return _activityRepository.Repository.InsertFacultySubMappings(facultySubjectMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual string UpdateFacultySubMapping(FacultySubjectMapping facultySubjectMapping)
        {
            try
            {
                return _activityRepository.Repository.UpdateFacultySubMapping(facultySubjectMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<FacultySubjectMapping>> DeleteFacultySubMapping(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteFacultySubMapping(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentMark>> UpdateReadytosendEmail(bool ReadytosendEmail)
        {
            try
            {
                return await _activityRepository.Repository.UpdateReadytosendEmail(ReadytosendEmail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<Announcement>> GetAnnouncementDetails(int? id, bool isReadToSendData)
        {
            try
            {
                return await _activityRepository.Repository.GetAnnouncementDetails(id, isReadToSendData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<Announcement> InsertAnnouncementDetails(Announcement announcement)
        {
            try
            {
                return await _activityRepository.Repository.InsertAnnouncementDetails(announcement);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public virtual async Task<List<Announcement>> DeleteAnnouncementDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteAnnouncementDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public virtual string DownloadFeedbackQnsReport(long departmentId, long subjectId, long facultyId, string sem, string year, string section)
        //{
        //    return _activityRepository.Repository.DownloadFeedbackQnsReport(departmentId, subjectId, facultyId, sem, year, section);

        //}

        //public virtual string DownloadFeedbackFacultyReport(long departmentId, string sem, string year, string section = null)
        //{
        //    return _activityRepository.Repository.DownloadFeedbackFacultyReport(departmentId, sem, year, section);

        //}

        public virtual string DeleteMark(List<StudentMark> mark)
        {
            try
            {
                return _activityRepository.Repository.DeleteMark(mark);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string DownloadSemWiseFeedbackReport(string departmentId, string sem, string year, string section, bool isSubmitted)
        {
            return _activityRepository.Repository.DownloadSemWiseFeedbackReport(departmentId, sem, year, section, isSubmitted);





        }
        public virtual async Task<List<StudentSemDateModel>> updateStudentSemDateDetails(StudentSemDateModel studentSemDate)
        //(string Sem, DateTime FirstYearStartDate, DateTime FirstYearEndDate, DateTime SecondYearStartDate, DateTime SecondYearEndDate,
        //DateTime ThirdYearStartDate, DateTime ThirdYearEndDate)
        {
            try
            {
                return await _activityRepository.Repository.updateStudentSemDateDetails(studentSemDate);
                //(Sem, FirstYearStartDate, FirstYearEndDate, SecondYearStartDate, SecondYearEndDate, ThirdYearStartDate, ThirdYearEndDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public Task<List<FacultySubjectMapping>> getFacultySubjectforfeedback(int Department, string Sem, string Year, string Section, int Subject)
        {
            try
            {
                return _activityRepository.Repository.getFacultySubjectforfeedback(Department, Sem, Year, Section, Subject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string UpdateStdconfigFeedback(long? Department, bool IsFeebackSend)
        {
            try
            {
                return _activityRepository.Repository.UpdateStdconfigFeedback(Department, IsFeebackSend);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentSemDateModel>> GetAllStudentConfiguration(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllStudentConfiguration(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual string GenerateMarkReport(string departmentId, string sem, string year, string section, string test)
        {
            return _activityRepository.Repository.GenerateMarkReport(departmentId, sem, year, section, test);

        }
        public virtual string SendStudentFeeedback(string StudentId)
        {
            try
            {
                return _activityRepository.Repository.SendStudentFeeedback(StudentId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentDropdown>> GetMappedStudentByName(string StudentName, int DepartmentId, string Sem, string Year)
        {
            try
            {
                return await _activityRepository.Repository.GetMappedStudentByName(StudentName, DepartmentId, Sem, Year);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public virtual string DownloadYearWiseSecFeedbackReport(long subjectId, string sem, string year)
        //{
        //    return _activityRepository.Repository.DownloadYearWiseSecFeedbackReport(subjectId, sem, year);

        //}
        public virtual string GenerateStdAssReports(int departmentId)
        {
            return _activityRepository.Repository.GenerateStdAssReports(departmentId);

        }
        public virtual async Task<List<MemberDetails>> GetMembersDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetMembersDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<MemberDetails>> InsertMembersDetails(MemberDetails memberDetails)
        {
            try
            {
                return await _activityRepository.Repository.InsertMembersDetails(memberDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual async Task<List<MemberDetails>> UpdateMembersDetails(MemberDetails memberDetails)
        {
            try
            {
                return await _activityRepository.Repository.UpdateMembersDetails(memberDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<MemberDetails>> DeleteMembersDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteMembersDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public virtual string SendWhatsAppMsg(long id, string formName, string receiverRoleName, string message, string senderName)
        {
            try
            {
                return _activityRepository.Repository.SendWhatsAppMsg(id, formName, receiverRoleName, message, senderName);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public virtual string UpdateAnnouncement(int id,  bool isReadyToSend)
        //{
        //    try
        //    {
        //        return _activityRepository.Repository.UpdateAnnouncement( id, isReadyToSend);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public virtual async Task<List<StudentMark>> GetStudentMarkByIdDetails(int studentId)
        {
            try
            {
                return await _activityRepository.Repository.GetStudentMarkByIdDetails(studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentAttendanceModel>> GetAttendanceByIdDetails(int studentId, int month, int year)
        {
            try
            {
                return await _activityRepository.Repository.GetAttendanceByIdDetails(studentId, month, year);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual async Task<List<AssignmentModel>> GetAllAssignmentDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllAssignmentDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AssignmentModel>> InsertAssignmentDetails(AssignmentModel assignmentModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertAssignmentDetails(assignmentModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AssignmentModel>> UpdateAssignmentDetails(AssignmentModel assignmentModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateAssignmentDetails(assignmentModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AssignmentModel>> DeleteAssignmentDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteAssignmentDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AssignmentModel>> GetAllAssignmentByStudentDetails(string role,int studentId)
        {
            try
            {
                return await _activityRepository.Repository.GetAllAssignmentByStudentDetails(role,studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<List<TimetableModel>> GetAllTimetableDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllTimetableDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<TimetableModel>> GetTimeTableBySectionIdDetails(int sectionId,string role)
        {
            try
            {
                return await _activityRepository.Repository.GetTimeTableBySectionIdDetails(sectionId,role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<TimetableModel>> InsertTimetableDetails(TimetableModel timetableModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertTimetableDetails(timetableModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<TimetableModel>> UpdateTimetableDetails(TimetableModel timetableModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateTimetableDetails(timetableModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<TimetableModel>> DeleteTimetableDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteTimetableDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<StudentFeedbackModel>> GetAllStudentFeedbackDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllStudentFeedbackDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> InsertStudentFeedbackDetails(List<StudentFeedbackModel> studentFeedbackModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertStudentFeedbackDetails(studentFeedbackModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public virtual async Task<int> UpdateStudentFeedbackDetails(StudentFeedbackModel studentFeedbackModel)
        //{
        //    try
        //    {
        //        return await _activityRepository.Repository.UpdateStudentFeedbackDetails(studentFeedbackModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public virtual async Task<List<StudentFeedbackModel>> DeleteStudentFeedbackDetails(string id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteStudentFeedbackDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UserFcmToken>>  SaveFcmToken(UserFcmToken model)
        {
            try
            {
                return await _activityRepository.Repository.SaveFcmToken(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<UserFcmToken>> GetUserDeviceToken(int studentId, string role)
        {
            try
            {
                return await _activityRepository.Repository.GetUserDeviceToken(studentId,role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<NotificationModel>> GetNotificationDetails(int studentId, string role)
        {
            try
            {
                return await _activityRepository.Repository.GetNotificationDetails(studentId, role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<NotificationModel>> UpdateNotificationDetails(NotificationModel notificationModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateNotificationDetails(notificationModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ContentLibModel>> GetAllContentLibDetails(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllContentLibDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ContentLibModel>> InsertContentLibDetails(ContentLibModel contentLibModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertContentLibDetails(contentLibModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ContentLibModel>> UpdateContentLibDetails(ContentLibModel contentLibModel)
        {
            try
            {
                return await _activityRepository.Repository.UpdateContentLibDetails(contentLibModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ContentLibModel>> DeleteContentLibDetails(int id)
        {
            try
            {
                return await _activityRepository.Repository.DeleteContentLibDetails(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<ContentLibModel>> GetAllContentLibByStudentDetails(int studentId)
        {
            try
            {
                return await _activityRepository.Repository.GetAllContentLibByStudentDetails(studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<BirthdayModel>> GetBirthdayListByRole(string role)
        {
            try
            {
                return await _activityRepository.Repository.GetBirthdayListByRole(role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> UpdateSubmitStatus(int studentId)
        {
            try
            {
                return await _activityRepository.Repository.UpdateSubmitStatus(studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<string> InsertFeedBackDetails(Feedback feedback)
        {
            try
            {
                return await _activityRepository.Repository.InsertFeedBackDetails(feedback);
                }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public virtual async Task<Feedback> getFeedbackDetails(int studentId, int subjectId, int facultyId)
        {
            try
            {
                return await _activityRepository.Repository.getFeedbackDetails(studentId,subjectId,facultyId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<Feedbacksubject>> getSubFacultyList(int studentId)
        {
            try
            {
                return await _activityRepository.Repository.getSubFacultyList(studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<Feedbacksubject>> checkFeedbackSubmittedAsync(int studentId)
        {
            try
            {
                return await _activityRepository.Repository.checkFeedbackSubmittedAsync(studentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual async Task<List<AcademicCalender>> GetAllAcademicCalender(string role)
        {
            try
            {
                return await _activityRepository.Repository.GetAllAcademicCalender(role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AcademicCalender>> InsertAcademicCalender(AcademicCalender academicCalender)
        {
            try
            {
                return await _activityRepository.Repository.InsertAcademicCalender(academicCalender);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AcademicCalender>> UpdateAcademicCalender(AcademicCalender academicCalender)
        {
            try
            {
                return await _activityRepository.Repository.UpdateAcademicCalender(academicCalender);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<AcademicCalender>> DeleteAcademicCalender(int SNo)
        {
            try
            {
                return await _activityRepository.Repository.DeleteAcademicCalender(SNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InfoGaloreModel>> GetAllInfoGalore(int? id)
        {
            try
            {
                return await _activityRepository.Repository.GetAllInfoGalore(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InfoGaloreModel>> InsertInfoGalore(InfoGaloreModel infoGaloreModel)
        {
            try
            {
                return await _activityRepository.Repository.InsertInfoGalore(infoGaloreModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual async Task<List<InfoGaloreModel>> UpdateInfoGalore(int id, string target)
        {
            try
            {
                return await _activityRepository.Repository.UpdateInfoGalore(id,target);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
