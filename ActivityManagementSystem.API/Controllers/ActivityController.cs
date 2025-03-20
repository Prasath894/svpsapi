﻿using ActivityManagementSystem.BLL;
using ActivityManagementSystem.BLL.Services;
using ActivityManagementSystem.Domain.AppSettings;
using ActivityManagementSystem.Domain.Models.Activity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.Office.Interop.Word;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;
using ActivityManagementSystem.BLL.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.SqlServer.Management.XEvent;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ActivityManagementSystem.Domain.Models;
using System.Net.Mail;
using ActivityManagementSystem.BLL.Common;
using System.Collections.Concurrent;

namespace ActivityManagementSystem.API.Controllers
{
    [Route("api/[Action]")]
    [ApiController]
    //[Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IServices<ActivityService> _activityService;
        private readonly ILogger<ActivityController> _logger;
        protected string Role => User.FindFirst("userRole")?.Value;

        // Get the UserId claim value
        protected int UserId => Convert.ToInt32(User.FindFirst("userId")?.Value);

        public ActivityController(ILogger<ActivityController> logger, AppSettings appSettings, IServices<ActivityService> activityService)
        {
            this._activityService = activityService;
            this._appSettings = appSettings;
            this._logger = logger;
            //_appSettings = appSettings;
        }
        protected IActionResult BadRequestError(Exception ex)
        {
            _logger.LogError("Error executing the request: {message}", ex.Message);
            var errorResponse = new { message = ex.Message };
            return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
        }

        protected IActionResult BadRequestError(string errorMessage)
        {
            var errorResponse = new { message = errorMessage };
            return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
        }

        protected IActionResult SuccessMessage(string message)
        {
            return StatusCode(StatusCodes.Status200OK, message);
        }
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Token))]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody][Required] LoginUserDto user)
        {
            if (!string.IsNullOrEmpty(user.Role.ToString()))
            {
                if (((user.Role.ToString() == "Parent") || (user.Role.ToString() == "Student")) && (string.IsNullOrEmpty(user.MobileNo)))
                {
                    return BadRequestError("Mobile number cannot be empty.");
                }
                if ((user.Role.ToString() == "Teacher" || (user.Role.ToString() == "Admin") || (user.Role.ToString() == "Principal")) && (string.IsNullOrEmpty(user.Username)))
                {
                    return BadRequestError("Username cannot be empty.");
                }
                if (((user.Role.ToString() == "Teacher") || (user.Role.ToString() == "Admin") || (user.Role.ToString() == "Principal")) && (string.IsNullOrEmpty(user.Password)))
                {
                    return BadRequestError("Password cannot be empty.");
                }
            }

            try
            {
                var token = await _activityService.Service.LoginAsync(user);
                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    return Unauthorized(new { message = "You are not allowed to login." });
                }
            }
            catch (Exception ex)
            {
                return BadRequestError(ex);
            }

        }

    
       

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FacultyDropdown))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFacultyByName(string FacultyName)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetFacultyByName(FacultyName);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
       




        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentDetails(int? Id = null)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetStudentDetails(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileNames;
                if (files != null)
                {
                    result[i].Files = files.Split('|').ToList();
                    result[i].Files.RemoveAt(result[i].Files.Count - 1);

                }
                //  List<string> lst = 
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertStudent([FromBody] StudentModel student)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertStudentDetails(student);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentModel student)
        {
            //StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.UpdateStudentDetails(student);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HouseModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllHouse(int? Id=null)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllHouse(Id);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


      
        
      

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertHouse([FromBody] HouseModel house)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertHouseDetails(house);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


       

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateHouse([FromBody] HouseModel house)
        {
            //StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.UpdateHouseDetails(house);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteHouse(int Id)
        {
            var result = _activityService.Service.DeleteHouseDetails(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStudent(int Id)
        {
            var result = await _activityService.Service.DeleteStudentDetails(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFaculty(int Id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetFacultyDetails(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllFaculty()
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetFacultyDetails(null);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }

            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileNames;
                if (files != null)
                {
                    result[i].files = files.Split('|').ToList();
                    result[i].files.RemoveAt(result[i].files.Count - 1);

                }
                //  List<string> lst = 
            }

            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertFaculty([FromBody] FacultyModel faculty)
        {
            //FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = _activityService.Service.InsertFacultyDetails(faculty);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ActivityModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllActivityData(int Type, long? DepartmentId)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetAllActivityData(Type, DepartmentId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }


            return Ok(result);
        }



        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ActivityModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetActivityData(int? id)
        {
            //_logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetActivityData(id);


            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(RoleActivity))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleActivity(int? id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetRoleActivity(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }



       
       

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ActivityModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertActivityData([FromBody] ActivityModel activitydata)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            //ActivityModel activity = JsonConvert.DeserializeObject<ActivityModel>(activitydata);


            var result = await _activityService.Service.InsertActivityData(activitydata);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ActivityModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateActivityData([FromBody] ActivityModel activitydata)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            //ActivityModel activity = JsonConvert.DeserializeObject<ActivityModel>(activitydata);


            var result = await _activityService.Service.UpdateActivityData(activitydata);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]

        [ProducesResponseType(200, Type = typeof(ActivityModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteActivityData(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteActivityData(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]       
        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        
        public async Task<IActionResult> GetAllRole()
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetRole(null);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRole(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetRole(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertRole([FromBody] RoleModel rolemaster)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            //RoleModel role = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = await _activityService.Service.InsertRole(rolemaster);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole([FromBody] RoleModel rolemaster)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            // RoleModel roll = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = await _activityService.Service.UpdateRole(rolemaster);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]

        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = _activityService.Service.DeleteRole(id);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFaculty([FromBody] FacultyModel faculty)
        {
            //FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = _activityService.Service.UpdateFacultyDetails(faculty);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFaculty(int Id)
        {
            var result = await _activityService.Service.DeleteFacultyDetails(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel fileUploadModel)
        {
            if (fileUploadModel.FormFiles != null)
            {
                //subDirectory = subDirectory ?? string.Empty;
                var target = Path.Combine(Directory.GetCurrentDirectory().ToString(), fileUploadModel.ActivityName, fileUploadModel.ActivityName + "-" + fileUploadModel.Id);

                //Path.Combine(_appSettings.Settings.UploadFilePath.ToString(), fileUploadModel.ActivityName, fileUploadModel.ActivityName + "-" + fileUploadModel.Id);
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
                for (int i = 0; i < fileUploadModel.FormFiles.Count; i++)
                {
                    string path = Path.Combine(target, fileUploadModel.FormFiles[i].FileName);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        await fileUploadModel.FormFiles[i].CopyToAsync(stream);
                    }
                }
                string filenames = "";
                if (target != "")
                {
                    string[] filePaths = Directory.GetFiles(target);
                    foreach (var file in filePaths)
                    {
                        filenames = filenames + Path.GetFileName(file) + "|";

                    }

                }
                var result = await _activityService.Service.UpdateActivityFilepathdata(target, fileUploadModel.Id, filenames);
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FileUpload))]
        [ProducesResponseType(404)]

        public async Task<IActionResult> UploadfacultystudentFile([FromForm] FileUpload fileUploadModel)
        {
            if (fileUploadModel.FormFiles != null)
            {
                //subDirectory = subDirectory ?? string.Empty;
                var target = Path.Combine(Directory.GetCurrentDirectory(), fileUploadModel.TypeofUser, fileUploadModel.TypeofUser + "-" + fileUploadModel.Id);

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
             //   Directory.CreateDirectory(target);


                for (int i = 0; i < fileUploadModel.FormFiles.Count; i++)
                {
                    string path = Path.Combine(target, fileUploadModel.FormFiles[i].FileName);
                    using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        await fileUploadModel.FormFiles[i].CopyToAsync(stream);
                    }
                }
                string filenames = "";
                if (target != "")
                {
                    string[] filePaths = Directory.GetFiles(target);
                    foreach (var file in filePaths)
                    {
                        filenames = filenames + Path.GetFileName(file) + "|";

                    }

                }
                var result = await _activityService.Service.UpdateFilepathdata(target, fileUploadModel.Id, filenames, fileUploadModel.TypeofUser);
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFiles(int id)
        {
            var result1 = await _activityService.Service.GetActivityData(id);
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            var filePath = result1[0].FilePath;
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();

            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {

                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });


            }
            const string contentType = "application/zip";
            HttpContext.Response.ContentType = contentType;
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;

        }

        [HttpGet]
        public async Task<IActionResult> DownloadStudentFiles(int id)
        {
            var result1 = await _activityService.Service.GetStudentDetails(id);
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            var filePath = result1[0].FilePath;
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();

            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {

                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });


            }
            const string contentType = "application/zip";
            HttpContext.Response.ContentType = contentType;
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;

        }

        

        [HttpGet]
        public async Task<IActionResult> DownloadFacultyFiles(int id)
        {
            // Get faculty details
            var result1 = await _activityService.Service.GetFacultyDetails(id);
            if (result1 == null || result1.Count == 0)
            {
                return NotFound("No faculty details found for the given ID.");
            }

            // Get file path
            var filePath = result1[0].FilePath;
            if (string.IsNullOrEmpty(filePath) || !Directory.Exists(filePath))
            {
                return NotFound("File path is invalid or does not exist.");
            }

            // Get list of files
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            if (files.Count == 0)
            {
                return NotFound("No files available for the given ID.");
            }

            // Prepare zip archive
            var zipName = $"archive-{DateTime.Now:yyyy_MM_dd-HH_mm_ss}";
            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {
                files.ForEach(file =>
                {
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });
            }

            compressedFileStream.Seek(0, SeekOrigin.Begin); // Reset stream position
            const string contentType = "application/zip";
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;
        }



        [HttpGet]
        public async Task<IActionResult> DownloadAnnouncementFiles(int id, bool? isReadToSendData = false)
        {
            var isReadToSend = isReadToSendData == null ? false : isReadToSendData;
            var result1 = await _activityService.Service.GetAnnouncementDetails(id, (bool)isReadToSend);
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}";
            var filePath = result1[0].Filepath;
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();


            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {

                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });


            }
            const string contentType = "application/zip";
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;

        }

        [HttpGet]
        public async Task<IActionResult> DownloadAssignmentFiles(int id)
        {
            var result1 = await _activityService.Service.GetAllAssignmentDetails(id);
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}";
            var filePath = result1[0].FilePath;
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();


            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {

                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });


            }
            const string contentType = "application/zip";
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;

        }
        [HttpGet]
        public async Task<IActionResult> DownloadContentLibFiles(int id)
        {
            var result1 = await _activityService.Service.GetAllContentLibDetails(id);
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}";
            var filePath = result1[0].FilePath;
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();


            MemoryStream compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {

                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });


            }
            const string contentType = "application/zip";
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;

        }

        //[HttpGet]
        //[ProducesResponseType(200, Type = typeof(DepartmentModel))]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetAllDepartment(int? id)
        //{
        //    var result = await _activityService.Service.GetAllDepartment(id);
        //    _logger.LogDebug(result.ToString());
        //    if (result == null)
        //    {
        //        return NoContent();
        //    }
        //    return Ok(result);
        //}


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(SubjectModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllSubject(int? Id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllSubject(Id);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SubjectModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertSubject([FromBody] SubjectModel subject)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            //RoleModel role = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = await _activityService.Service.InsertSubjectDetails(subject);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SubjectModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectModel subject)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            // RoleModel roll = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = await _activityService.Service.UpdateSubjectDetails(subject);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]

        [ProducesResponseType(200, Type = typeof(SubjectModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = _activityService.Service.DeleteSubjectDetails(id);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(SectionModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllSectionDetails(int? id)
        {
            var result = await _activityService.Service.GetAllSectiones(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SectionModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertSectionDetails(SectionModel section)
        {
            var result = await _activityService.Service.InsertSectionDetails(section);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(SectionModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSectionDetails([FromBody] SectionModel section)
        {
            var result = await _activityService.Service.UpdateSectionDetails(section);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]

        [ProducesResponseType(200, Type = typeof(SectionModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSectionDetails(int id)
        {
            var result = await _activityService.Service.DeleteSectionDetails(id);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BatchStudMappingModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllSectionStudMappingDetails(int? id)
        {
            var result = await _activityService.Service.GetAllSectionStudMappings(id);
            var groupByData = result.GroupBy(x => x.SectionId);
            var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(jsonData);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchStudMappingModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertSectionStudMappingDetails(List<BatchStudMappingModel> data)
        {
            var result = await _activityService.Service.InsertSectionStudMappings(data);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchStudMappingModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSectionStudMapping([FromBody] List<BatchStudMappingModel> data)
        {
            var result = await _activityService.Service.UpdateSectionStudMapping(data);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchStudMappingModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteSectionStudMapping(int[] ids, int batchId)
        {
            var result = await _activityService.Service.DeleteSectionStudMapping(ids, batchId);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> bulkupload([FromForm] ExcelUpload fileUploadModel)
        {
            var result = "";
            if (fileUploadModel.FormFiles != null)
            {

                (string filePath, string fileName) = await FileOperations.SaveFileWithTimeStamp(fileUploadModel);
              
                var XlPath = fileUploadModel.FormFiles.FileName;
                if (XlPath == "Student.csv")
                {
                    var uploadedFileData = DataTableConverter.ConvertCsvToDataTable(filePath);
                    result = await _activityService.Service.bulkuploadstudent(uploadedFileData);

                }
                else if (XlPath == "Faculty.csv")
                {
                    var uploadedFileData = DataTableConverter.ConvertCsvToDataTable(filePath);
                    result = await _activityService.Service.bulkuploadfaculty(uploadedFileData);
                }
               
                else if (XlPath == "Subject.csv")
                {
                    var uploadedFileData = DataTableConverter.ConvertCsvToDataTable(filePath);
                    result = await _activityService.Service.bulkuploadsubject(uploadedFileData);
                }
                else if (XlPath == "Mark.xlsx")
                {
                    result = await _activityService.Service.bulkuploadmark(filePath, fileUploadModel.Section);
                }
                else if (XlPath == "HolidayCalendar.csv")
                {
                    var uploadedFileData = DataTableConverter.ConvertCsvToDataTable(filePath);
                    result = await _activityService.Service.bulkuploadholidaycalendar(uploadedFileData);
                }

            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Template");
            var files = Directory.GetFiles(Path.Combine(filePath)).ToList();
            MemoryStream compressedFileStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
            {
                files.ForEach(file =>
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(file));
                    byte[] bytes = System.IO.File.ReadAllBytes(file);
                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(bytes))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                });
            }
            const string contentType = "application/zip";
            HttpContext.Response.ContentType = contentType;
            var result = new FileContentResult(compressedFileStream.ToArray(), contentType)
            {
                FileDownloadName = $"{zipName}.zip"
            };
            return result;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AttendanceModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllAttendance(DateTime? AttendanceDate,   int sectionId,  string Hoursday)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllAttendance(AttendanceDate,   sectionId,Hoursday);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AttendanceModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertAttendance([FromBody] List<AttendanceModel> attendance)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            //RoleModel role = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = _activityService.Service.InsertAttendance(attendance);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AttendanceModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAttendance([FromBody] AttendanceModel attendance)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());

            // RoleModel roll = JsonConvert.DeserializeObject<RoleModel>(rolemaster);


            var result = await _activityService.Service.UpdateAttendance(attendance);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AttendanceModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAttendance([FromBody] List<AttendanceModel> attendance)

        {
            var result = _activityService.Service.DeleteAttendance(attendance);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AttachmentModel>))]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> GetAttachment(int id, string type)
        {

            _logger.LogInformation("{MethodName} method is called", nameof(GetAttachment));
            try
            {

                var attachmentData = await _activityService.Service.GetAttachmentAsync(id, type);
                return Ok(attachmentData);
            }
            catch (InvalidDataException)
            {
                return Ok("Success: No valid file found.");
            }           

        }
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> DownloadAttachment(int id, string type, string filename)
        {
            _logger.LogInformation("{MethodName} method is called", nameof(DownloadAttachment));
            try
            {
                // Construct file path

                string filePath =(type!="InfoGalore") ? Path.Combine(Directory.GetCurrentDirectory(), "Attachments", type, $"{type}-{id}", filename):
                    Path.Combine(Directory.GetCurrentDirectory(), "Attachments", type, $"{type}-", filename);
                _logger.LogInformation("File path: {FilePath}", filePath);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    return NotFound("File not found.");
                }

                // Prepare memory stream
                MemoryStream memory = new MemoryStream();

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                // Get content type
                string contentType;
                try
                {
                    contentType = FindContentType.GetContentType(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to determine content type");
                    return StatusCode(500, "Failed to determine content type.");
                }

                // Reset memory stream position
                memory.Position = 0;

                return File(memory, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while downloading the attachment");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ActivityFilterModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReports([FromBody] ActivityFilterModel activityFilterModel)
        {

            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            switch (activityFilterModel.Type)
            {
                case 1:

                    return Ok(await _activityService.Service.GetAllActivityDataForReport(activityFilterModel));

                case 2:
                    return Ok(await _activityService.Service.GetProjectModelForReport(activityFilterModel));

                case 3:
                    return Ok(await _activityService.Service.GetGuestlecturesForReport(activityFilterModel));

                case 10:
                    return Ok(await _activityService.Service.GetImplantTrainingReport(activityFilterModel));

                case 12:
                    return Ok(await _activityService.Service.GetAllIndustrialVisitDataForReport(activityFilterModel));

                case 13:
                    return Ok(await _activityService.Service.GetSportsGamesReport(activityFilterModel));

                case 14:
                    return Ok(await _activityService.Service.GetAllNccDataForReport(activityFilterModel));

                case 15:
                    return Ok(await _activityService.Service.GetNSSReport(activityFilterModel));

                case 16:
                    return Ok(await _activityService.Service.GetFacultyDevelopmentReport(activityFilterModel));

                case 18:
                    return Ok(await _activityService.Service.GetAllConsultantDataForReport(activityFilterModel));

                case 20:
                    return Ok(await _activityService.Service.GetJournalForReport(activityFilterModel));

                case 23:
                    return Ok(await _activityService.Service.GetWomenDevelopmentReport(activityFilterModel));

                case 25:
                    return Ok(await _activityService.Service.GetAllPatentDataForReport(activityFilterModel));

                case 26:
                    return Ok(await _activityService.Service.GetSemesterResultReport(activityFilterModel));
                case 27:
                    return Ok(await _activityService.Service.GetSymposiumResultReport(activityFilterModel));
                case 29:
                    return Ok(await _activityService.Service.GetPlacementResultReport(activityFilterModel));
                //case 30:
                //    return Ok(await _activityService.Service.GetAllActivityDataForReport(activityFilterModel));

                case 31:
                    return Ok(await _activityService.Service.GetAllExtensionServicesDataForReport(activityFilterModel));
                case 32:
                    return Ok(await _activityService.Service.GetAllGrantsDataForReport(activityFilterModel));

                case 34:
                    return Ok(await _activityService.Service.GetAllMiscellaneousDataForReport(activityFilterModel));
                case 37:
                    return Ok(await _activityService.Service.GetAllAwardsDataForReport(activityFilterModel));
                case 39:
                    return Ok(await _activityService.Service.GetAllEventsDataForReport(activityFilterModel));
                case 41:
                    return Ok(await _activityService.Service.GetAllMOUsDataForReport(activityFilterModel));
                case 45:
                    return Ok(await _activityService.Service.GetAllAdDataForReport(activityFilterModel));
                case 46:
                    return Ok(await _activityService.Service.GetAllPressDataForReport(activityFilterModel));
                case 48:
                    return Ok(await _activityService.Service.GetAllUpcomingEventsDataForReport(activityFilterModel));


            }
            //_logger.LogDebug(result.ToString());

            // if (result == null)

            //    return NoContent();
            //}
            //return Ok(result);
            return NotFound();
        }

        //[HttpGet]
        //[ProducesResponseType(200, Type = typeof(BatchStudentMapping))]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetBatchWiseStudentList()
        //{
        //    //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
        //    //$"\r\n product subcategories", ToString());
        //    var result = await _activityService.Service.GetBatchWiseStudentList();
        //    //var sentBack = result.GroupBy(x => x.BatchId);

        //    _logger.LogDebug(result.ToString());
        //    if (result == null)
        //    {
        //        return NoContent();
        //    }
        //    return Ok(result);
        //}

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PasswordReset(string userName, string password)
        {
            var result = await _activityService.Service.PasswordReset(userName, password);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }

        //[HttpPost]
        //[ProducesResponseType(200, Type = typeof(List<string>))]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetSection(SectionModel sectionModel)
        //{

        //    var result = await _activityService.Service.GetSection(sectionModel);
        //    _logger.LogDebug(result.ToString());
        //    if (result == null)
        //    {
        //        return NoContent();
        //    }
        //    return Ok(result);
        //}
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Batchdetails))]
        [ProducesResponseType(404)]

        public async Task<IActionResult> GetBatchList(BatchListModel batchListModel)
        {
            var result = await _activityService.Service.GetBatchList(batchListModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BatchSubjectFacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllBatchSubMappingDetails(int? id)
        {
            var result = await _activityService.Service.GetAllBatchSubMappings(id);
            
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchSubjectFacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertBatchSubMappingDetails(BatchSubjectFacultyModel data)
        {
            var result = await _activityService.Service.InsertBatchSubMappings(data);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchSubjectFacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBatchSubMapping([FromBody] BatchSubjectFacultyModel data)
        {
            var result = await _activityService.Service.UpdateBatchSubMapping(data);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(BatchSubjectFacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteBatchSubMapping(int ids)
        {
            var result = _activityService.Service.DeleteBatchSubMapping(ids);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        //[HttpPost]
        //public  IActionResult  generateAttendancereport()
        //{
        //    var result =  _activityService.Service.generateAttendancereport();

        //    return Ok(result);
        //}
       

        
        [HttpGet]
        //[EnableCors]
        // [EnableCors(origin:"http://103.53.52.215", Headers:"*")]
        public async Task<FileResult> generateAttendanceMonthwisereport(int startMonth, int startYear, int endMonth, int endYear, int sectionId, string grade, string section)
        {
            try
            {


                var result = _activityService.Service.generateMonthlyAttendancereport(startMonth, startYear, endMonth, endYear, sectionId,grade,section);

                _logger.LogDebug(result.ToString());
                return await PrepareFileForDownload(result.ToString(), "Excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpGet]
       /// [EnableCors]
        // [EnableCors(origin:"http://103.53.52.215", Headers:"*")]
        public async Task<FileResult> generateExcelList(string role)
        {
            try
            {


                var result = _activityService.Service.generateExcelList(role);

                _logger.LogDebug(result.ToString());
                return await PrepareFileForDownload(result.ToString(), "Excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
       
        [HttpGet]
        //[EnableCors]
        // [EnableCors(origin:"http://103.53.52.215", Headers:"*")]
        public async Task<FileResult> generateDailyAttendancereport(int month, int year, int sectionId, string grade, string section)
        {
            try
            {


                var result = _activityService.Service.generateDailyAttendancereport(month, year, sectionId, grade, section);

                _logger.LogDebug(result.ToString());
                return await PrepareFileForDownload(result.ToString(), "Excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }




        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetVerifyPassword(string UserName, string Password)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetVerifyPassword(UserName, Password);



            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateVerifyPassword(string UserName, string NewPassword, string OldPassword, long FacultyId)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = _activityService.Service.UpdateVerifyPassword(UserName, NewPassword, OldPassword, FacultyId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
       
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(RoleModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllExams()
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetExams(null);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ExamsModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetExams(int? id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetExams(id);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ExamsModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertExams([FromBody] ExamsModel roleModel)
        {
            var result = await _activityService.Service.InsertExams(roleModel);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ExamsModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateExams([FromBody] ExamsModel roleModel)
        {
            var result = await _activityService.Service.UpdateExams(roleModel);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ExamsModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteExams(int id)
        {
            var result = await _activityService.Service.DeleteExams(id);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HousePointModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetHousePoint()
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetHousePointDetails();
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HouseActivity))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetHouseActivity(int? id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetHouseActivity(id);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseActivity))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertHouseActivity([FromBody] HouseActivity model)
        {
            var result = await _activityService.Service.InsertHouseActivity(model);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseActivity))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateHouseActivity([FromBody] HouseActivity model)
        {
            var result = await _activityService.Service.UpdateHouseActivity(model);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HouseActivity))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteHouseActivity(int id)
        {
            var result = await _activityService.Service.DeleteHouseActivity(id);
            _logger.LogDebug(result.ToString());
            //if (result == null)
            //{
            //    return NoContent();
            //}
            return Ok(result);
        }
      

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(UpcomingCompetition))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllUpcomingCompetition(string role,int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllUpcomingCompetition(role,id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileNames;
                if (files != null)
                {
                    result[i].Files = files.Split('|').ToList();
                    result[i].Files.RemoveAt(result[i].Files.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UpcomingCompetition))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertUpcomingCompetition([FromBody] UpcomingCompetition model)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertUpcomingCompetition(model);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UpcomingCompetition))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUpcomingCompetition([FromBody] UpcomingCompetition model)
        {
            var result = await _activityService.Service.UpdateUpcomingCompetition(model);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UpcomingCompetition))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateInterestedCompetition(int studentId,int competitionId)
        {
            var result = await _activityService.Service.UpdateInterestedCompetition(studentId, competitionId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UpcomingCompetition))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUpcomingCompetition(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteUpcomingCompetition(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(LeaveModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllLeave(string role, int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllLeave(role, id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i<result.Count; i++)
            {
                var files = result[i].Filename;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(LeaveModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertLeave([FromBody] LeaveModel model)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertLeave(model);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(LeaveModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateLeave([FromBody] LeaveModel model)
        {
            var result = await _activityService.Service.UpdateLeave(model);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
       
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(LeaveModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteLeave(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteLeave(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
       // [AllowAnonymous]
        public async Task<FileResult> GetAllMarkReport(string Section, string subjects,string test)
        {
            try
            {

                var result = _activityService.Service.GetAllMarkReport( Section, subjects, test);
                //_logger.LogDebug(result.ToString());
                return await PrepareFileForDownload(result.ToString(), "Excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpGet]
        // [AllowAnonymous]
        public async Task<FileResult> GetInterestedStudentList(int competitionId)
        {
            try
            {

                var result = _activityService.Service.GetInterestedStudentList(competitionId);
                //_logger.LogDebug(result.ToString());
                return await PrepareFileForDownload(result.ToString(), "Excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentMark))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentMark()
        {
            //List<MarkDetails>= JsonConvert.DeserializeObject(studentmark.Data.ToString());
            //var items = JsonConvert.DeserializeObject<List<MarkDetails>>(studentmark.Data.ToString());            

            var result = _activityService.Service.GetStudentMark();

            //var data = result.Select(x => JsonConvert.DeserializeObject<StudentMark>(x.Data)).ToList();

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentMark))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertStudentMark([FromBody] StudentMark studentmark)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertStudentMark(studentmark);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();

            }
            return Ok(result);
        }
        [HttpGet]



        [ProducesResponseType(200, Type = typeof(FacultySubjectMapping))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllFacultySubMappingDetails(int? id)
        {
            var result = await _activityService.Service.GetAllFacultySubMappings(id);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultySubjectMapping))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertFacultySubMappingDetails([FromBody] FacultySubjectMapping facultySubjectMapping)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +

            //$"\r\n product subcategories", ToString());

            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = _activityService.Service.InsertFacultySubMappings(facultySubjectMapping);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();

            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultySubjectMapping))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFacultySubMapping([FromBody] FacultySubjectMapping facultySubjectMapping)
        {
            var result = _activityService.Service.UpdateFacultySubMapping(facultySubjectMapping);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(FacultySubjectMapping))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFacultySubMapping(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteFacultySubMapping(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentMark))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReadytosendEmail(bool ReadytosendEmail)
        {
            var result = await _activityService.Service.UpdateReadytosendEmail(ReadytosendEmail);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);



        }
        [HttpGet]
        public String translate(String EnglishTranslate, string From, string To)
        {
            var fromLanguage = From;
            var toLanguage = To;
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(EnglishTranslate)}";
            var webclient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webclient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4
                    , StringComparison.Ordinal) - 4);
                return result;
            }
            catch (Exception e1)
            {
                return "error";
            }

        }



        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Announcement))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAnnouncementDetails(int? id, bool isReadToSendData = false)
        {
            var result = await _activityService.Service.GetAnnouncementDetails(id, isReadToSendData);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileNames;
                if (files != null)
                {
                    result[i].Files = files.Split('|').ToList();
                    result[i].Files.RemoveAt(result[i].Files.Count - 1);

                }
                //  List<string> lst = 
            }
            
            return Ok(result);
        }
        //[HttpPost]
        //[ProducesResponseType(200, Type = typeof(FileUpload))]
        //[ProducesResponseType(404)]

        //public async Task<IActionResult> UpdateAnnouncement(int id,bool isReadyToSend)
        //{

        //    var result = _activityService.Service.UpdateAnnouncement(id, isReadyToSend);
        //    return Ok();

        //}


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Announcement))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertAnnouncementDetails([FromBody] Announcement announcement)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertAnnouncementDetails(announcement);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();



            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Announcement))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAnnouncementDetails(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteAnnouncementDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
      
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentMark))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMark([FromBody] List<StudentMark> mark)
        {
            var result = _activityService.Service.DeleteMark(mark);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(StudentSemDateModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> updateStudentSemDateDetails(StudentSemDateModel studentSemDate)
        //--(string Sem, DateTime FirstYearStartDate, DateTime FirstYearEndDate, DateTime SecondYearStartDate, DateTime SecondYearEndDate,
        // DateTime ThirdYearStartDate, DateTime ThirdYearEndDate)
        {

            var result = await _activityService.Service.updateStudentSemDateDetails(studentSemDate);
            //(Sem , FirstYearStartDate, FirstYearEndDate, SecondYearStartDate, SecondYearEndDate, ThirdYearStartDate, ThirdYearEndDate);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
       
      
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentSemDateModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllStudentConfiguration(int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllStudentConfiguration(id);



            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
       
       

        

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentMark))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetStudentMarkById(int studentId)
        {
            var result = await _activityService.Service.GetStudentMarkByIdDetails(studentId);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentAttendanceModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAttendanceById(int studentId, int month, int year)
        {
            var result = await _activityService.Service.GetAttendanceByIdDetails(studentId, month, year);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentDropdownModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMappedStudentByName(string StudentName, int SectionId)
        {
            var result = await _activityService.Service.GetMappedStudentByName(StudentName, SectionId);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AssignmentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllAssignment(int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllAssignmentDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileName;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AssignmentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertAssignment([FromBody] AssignmentModel assignmentModel)
        {

            var result = await _activityService.Service.InsertAssignmentDetails(assignmentModel);


            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            AssignmentModel assignment = result.FirstOrDefault();
            if (assignment != null && assignment.FileName != null)
            {
                string files = assignment.FileName;

                assignment.FileList = files.Split('|').ToList();
                assignment.FileList.RemoveAt(assignment.FileList.Count - 1);
            }
            return Ok(assignment);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AssignmentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAssignment([FromBody] AssignmentModel assignmentModel)
        {
            var result = await _activityService.Service.UpdateAssignmentDetails(assignmentModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AssignmentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteAssignmentDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AssignmentModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllAssignmentByStudent(string role,int studentId)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllAssignmentByStudentDetails(role,studentId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileName;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(TimetableModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllTimetable(int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllTimetableDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(TimetableModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTimeTableBySectionId(int sectionId,string role)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetTimeTableBySectionIdDetails(sectionId,role);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BatchSubjectFacultyModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFacultyListBySectionId(int sectionId)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetFacultyListBySectionIdDetails(sectionId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(TimetableModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertTimetable([FromBody] TimetableModel timetableModel)
        {

            var result = await _activityService.Service.InsertTimetableDetails(timetableModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(TimetableModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTimetable([FromBody] TimetableModel timetableModel)
        {
            var result = await _activityService.Service.UpdateTimetableDetails(timetableModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(TimetableModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTimetable(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteTimetableDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentFeedbackModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllStudentFeedback(int? id, string role)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllStudentFeedbackDetails(id,role);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileName;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(List<StudentFeedbackModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertStudentFeedback([FromBody] List<StudentFeedbackModel> studentFeedbackModel)
        {

            var result = await _activityService.Service.InsertStudentFeedbackDetails(studentFeedbackModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        //[HttpPost]
        //[ProducesResponseType(200, Type = typeof(StudentFeedbackModel))]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> UpdateStudentFeedback([FromBody] StudentFeedbackModel studentFeedbackModel)
        //{
        //    var result = await _activityService.Service.UpdateStudentFeedbackDetails(studentFeedbackModel);
        //    _logger.LogDebug(result.ToString());
        //    if (result == null)
        //    {
        //        return NoContent();
        //    }
        //    return Ok(result);

        //}
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(StudentFeedbackModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStudentFeedback(string id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteStudentFeedbackDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SaveFcmToken([FromBody] UserFcmToken model)
        {
            if (model == null || string.IsNullOrEmpty(model.DeviceToken))
                return BadRequest("Invalid token data");
            var result =await _activityService.Service.SaveFcmToken(model);
          

            return Ok("FCM token saved successfully.");
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(NotificationModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNotification(int studentId, string role)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetNotificationDetails(studentId, role);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(NotificationModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFireNotification(int studentId, string role)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetNotificationDetails(studentId, role);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            var deviceToken = await _activityService.Service.GetUserDeviceToken(studentId,role);
            if (string.IsNullOrEmpty(deviceToken.FirstOrDefault().DeviceToken.ToString()))
                return NotFound("User device token not found.");

            string fcmUrl = "https://fcm.googleapis.com/fcm/send";
            string serverKey = "YOUR_FIREBASE_SERVER_KEY"; // Get from Firebase Console

            var payload = new
            {
                to = deviceToken,
                notification = new
                {
                    title = result.FirstOrDefault().MsgType,
                    body = result.FirstOrDefault().NotificationMsg,
                    sound = "default"
                },
                data = new
                {
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    customData = "Additional Info"
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(fcmUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode ? Ok(responseString) : StatusCode((int)response.StatusCode, responseString);
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(NotificationModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateNotification(NotificationModel notificationModel)
        {

            var result = await _activityService.Service.UpdateNotificationDetails(notificationModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<ContentLibModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllContentLib(int? id)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllContentLibDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileName;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }

            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ContentLibModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertContentLib([FromBody] ContentLibModel contentLibModel)
        {

            var result = await _activityService.Service.InsertContentLibDetails(contentLibModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            ContentLibModel contentLib = result.FirstOrDefault();
            if (contentLib != null && contentLib.FileName != null)
            {
                string files = contentLib.FileName;

                contentLib.FileList = files.Split('|').ToList();
                contentLib.FileList.RemoveAt(contentLib.FileList.Count - 1);
            }
            return Ok(contentLib);

        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ContentLibModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateContentLib([FromBody] ContentLibModel contentLibModel)
        {
            var result = await _activityService.Service.UpdateContentLibDetails(contentLibModel);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ContentLibModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteContentLib(int id)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteContentLibDetails(id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ContentLibModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllContentLibByStudent(int studentId)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetAllContentLibByStudentDetails(studentId);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileName;
                if (files != null)
                {
                    result[i].FileList = files.Split('|').ToList();
                    result[i].FileList.RemoveAt(result[i].FileList.Count - 1);
                }
            }
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(BirthdayModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBirthdayListByRole(string role)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.GetBirthdayListByRole(role);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        private async Task<FileResult> PrepareFileForDownload(string result, string type)
        {
            try
            {
                var downloadData = await _activityService.Service.DownloadData(result);
                if (downloadData.memory == null || string.IsNullOrEmpty(downloadData.path))
                {
                    return null;
                }

                downloadData.memory.Position = 0;
                string contentType = string.Empty;
                if (type == "Word") contentType = "application/vnd.ms-word";
                else if (type == "Excel") contentType = "application/vnd.ms-excel";
                else contentType = "application/pdf";

                // Reading the file bytes and converting to Base64 (if needed for some reason)
                byte[] fileBytes = System.IO.File.ReadAllBytes(result);
                string base64String = Convert.ToBase64String(fileBytes);

                // Returning the file as a downloadable result
                return File(downloadData.memory, contentType, Path.GetFileName(downloadData.path));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error preparing file for download: {ex.Message}");
                throw;
            }
        }
       
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Feedbacksubject))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> getSubFacultyList(int studentId)
        {
            // FacultyModel facultyDetails = JsonConvert.DeserializeObject<FacultyModel>(faculty);
            var result = await _activityService.Service.getSubFacultyList(studentId);

            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AcademicCalender))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllAcademicCalender(string role)
        {
            var result = await _activityService.Service.GetAllAcademicCalender(role);
            //var groupByData = result.GroupBy(x => x.FacultyId);
            // var jsonData = JsonConvert.SerializeObject(groupByData);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AcademicCalender))]
        [ProducesResponseType(404)]

        public async Task<IActionResult> InsertAcademicCalender(AcademicCalender academicCalender)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertAcademicCalender(academicCalender);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();


            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AcademicCalender))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAcademicCalender(AcademicCalender academicCalender)
        {
            var result = await _activityService.Service.UpdateAcademicCalender(academicCalender);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AcademicCalender))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAcademicCalender(int SNo)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.DeleteAcademicCalender(SNo);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpGet]      
        public async Task<IActionResult> GetAllInfoGalore(string infoType,int? id)
        {


                var result = await _activityService.Service.GetAllInfoGalore(infoType, id);

                _logger.LogDebug(JsonConvert.SerializeObject(result));

                if (result == null || !result.Any())
                {
                    return NoContent();
                }

                string destination = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", "InfoGalore", $"InfoGalore-{id}");

                // Ensure the directory exists
                Directory.CreateDirectory(destination);

                var extractedFiles = new ConcurrentBag<InfoAttachmentModel>();

                // Process each entry in parallel
                await Parallel.ForEachAsync(result, async (entry, _) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(entry.InfoFilePath) && Directory.Exists(entry.InfoFilePath))
                        {
                            var files = Directory.GetFiles(entry.InfoFilePath);

                            foreach (var file in files)
                            {
                                var fileName = Path.GetFileName(file);
                                var destFile = Path.Combine(destination, fileName);

                                System.IO.File.Copy(file, destFile, true);

                                var attachment = new InfoAttachmentModel
                                {
                                    FileName = fileName,
                                    FilePath = destFile,
                                    InfoType = entry.InfoType
                                };

                                if (FileIsAnImageChecker.IsImageFile(file))
                                {
                                    attachment.BlobData = await System.IO.File.ReadAllBytesAsync(file);
                                }

                                extractedFiles.Add(attachment);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing files for entry {entry.InfoFilePath}: {ex.Message}");
                    }
                });

                return Ok(extractedFiles);
            }

        

        [HttpPost] 
        public async Task<IActionResult> InsertInfoGalore([FromForm] InfoGaloreModel infoGaloreModel)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);

            var result = await _activityService.Service.InsertInfoGalore(infoGaloreModel);
            if (infoGaloreModel.InfoFile != null)
            {
                //subDirectory = subDirectory ?? string.Empty;
                var target = Path.Combine(Directory.GetCurrentDirectory().ToString(), "InfoGalore", "InfoGalore" + "-" + result.FirstOrDefault().Id);

                //Path.Combine(_appSettings.Settings.UploadFilePath.ToString(), fileUploadModel.ActivityName, fileUploadModel.ActivityName + "-" + fileUploadModel.Id);
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
                for (int i = 0; i < infoGaloreModel.InfoFile.Count; i++)
                {
                    string path = Path.Combine(target, infoGaloreModel.InfoFile[i].FileName);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        await infoGaloreModel.InfoFile[i].CopyToAsync(stream);
                    }
                }
                string filenames = "";
                if (target != "")
                {
                    string[] filePaths = Directory.GetFiles(target);
                    foreach (var file in filePaths)
                    {
                        filenames = filenames + Path.GetFileName(file) + "|";

                    }

                }
                _logger.LogDebug(result.ToString());
                var infor =await _activityService.Service.UpdateInfoGalore(result.FirstOrDefault().Id, target);
                if (result == null)
                {
                    return NoContent();


                }
              
            }
            return Ok();
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HolidayCalendar))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetHolidayCalendar(int? Id = null)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            var result = await _activityService.Service.GetHolidayCalendar(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            for (int i = 0; i < result.Count; i++)
            {
                var files = result[i].FileNames;
                if (files != null)
                {
                    result[i].Files = files.Split('|').ToList();
                    result[i].Files.RemoveAt(result[i].Files.Count - 1);

                }
                //  List<string> lst = 
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HolidayCalendar))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InsertHolidayCalendar([FromBody] HolidayCalendar holiday)
        {
            //   _logger.LogDebug($" at product sub categories {{@this}} in Get method." +
            //$"\r\n product subcategories", ToString());
            // StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.InsertHolidayCalendar(holiday);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HolidayCalendar))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateHolidayCalendar([FromBody] HolidayCalendar holiday)
        {
            //StudentModel studentDetails = JsonConvert.DeserializeObject<StudentModel>(student);
            var result = await _activityService.Service.UpdateHolidayCalendar(holiday);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(HolidayCalendar))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteHolidayCalendar(int Id)
        {
            var result = await _activityService.Service.DeleteHolidayCalendar(Id);
            _logger.LogDebug(result.ToString());
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
    }
