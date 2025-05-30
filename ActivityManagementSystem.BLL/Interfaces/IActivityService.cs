using ActivityManagementSystem.Domain.Models.Activity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.BLL.Interfaces
{
    public interface IActivityService
    {

        Task<List<StudentModel>> GetStudentDetails(int? Id);
        Task<List<StudentModel>> InsertStudentDetails(StudentModel studentDetails);
        Task<List<StudentModel>> UpdateStudentDetails(StudentModel studentDetails);
        Task<List<StudentModel>> DeleteStudentDetails(int Id);


        Task<List<SectionModel>> GetAllSectiones(int? Id);
        Task<List<SectionModel>> InsertSectionDetails(SectionModel sectionModel);
        Task<List<SectionModel>> UpdateSectionDetails(SectionModel sectionModel);
        Task<List<SectionModel>> DeleteSectionDetails(int Id);

        Task<List<FacultyDropdown>> GetFacultyByName(string facultyName);

        Task<List<HouseModel>> GetAllHouse(int? Id);
        Task<List<HouseModel>> InsertHouseDetails(HouseModel house);
        Task<List<HouseModel>> UpdateHouseDetails(HouseModel house);
        Task<List<HouseModel>> DeleteHouseDetails(int id);

        Task<List<HousePointModel>> GetHousePointDetails();
        Task<List<HouseActivity>> GetHouseActivity(int? id);
        Task<List<HouseActivity>> InsertHouseActivity(HouseActivity roleModel);
        Task<List<HouseActivity>> UpdateHouseActivity(HouseActivity roleModel);
        Task<string> DeleteHouseActivity(int id);
        Task<List<ExamsModel>> GetExams(int? id);
        Task<List<ExamsModel>> InsertExams(ExamsModel roleModel);
        Task<List<ExamsModel>> UpdateExams(ExamsModel roleModel);
        Task<string> DeleteExams(int id);
        Task<List<ActivityModel>> GetAllActivityData(int Type, long? DepartmentId);
        Task<List<ActivityResponse>> GetAllActivityDataForReport(ActivityFilterModel activityFilterModel);

        Task<List<NccResponse>> GetAllNccDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<IndustrialVisitResponse>> GetAllIndustrialVisitDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<PatentResponse>> GetAllPatentDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<GrantsResponse>> GetAllGrantsDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<MiscellaneousResponse>> GetAllMiscellaneousDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<ExtensionServicesResponse>> GetAllExtensionServicesDataForReport(ActivityFilterModel activityFilterModel);
        Task<List<ConsultantResponse>> GetAllConsultantDataForReport(ActivityFilterModel activityFilterModel);
        Task<string> bulkuploadmark(string target,  string section);

        Task<List<ProjectModelResponse>> GetProjectModelForReport(ActivityFilterModel activityFilterModel);
        Task<List<ImplantTrainingResponse>> GetImplantTrainingReport(ActivityFilterModel activityFilterModel);
        Task<List<SportsandGamesResponse>> GetSportsGamesReport(ActivityFilterModel activityFilterModel);
        Task<List<NSSResponse>> GetNSSReport(ActivityFilterModel activityFilterModel);
        Task<List<FacultyDevelopmentResponse>> GetFacultyDevelopmentReport(ActivityFilterModel activityFilterModel);
        Task<List<WomenDevelopementResposnse>> GetWomenDevelopmentReport(ActivityFilterModel activityFilterModel);
        Task<List<SemesterResultResponse>> GetSemesterResultReport(ActivityFilterModel activityFilterModel);
        Task<List<SymposiumExpoResponse>> GetSymposiumResultReport(ActivityFilterModel activityFilterModel);
        Task<List<PlacementResponse>> GetPlacementResultReport(ActivityFilterModel activityFilterModel);


        Task<List<ActivityGuestlecturesResponse>> GetGuestlecturesForReport(ActivityFilterModel activityFilterModel);
        Task<List<JournalResponse>> GetJournalForReport(ActivityFilterModel activityFilterModel);
        Task<List<ActivityModel>> GetActivityData(int? id);
        Task<List<ActivityModel>> InsertActivityData(ActivityModel activitydetails);
        Task<List<ActivityModel>> UpdateActivityData(ActivityModel activitydetails);
        Task<List<ActivityModel>> DeleteActivityData(int id);
        
        Task<List<RoleModel>> GetRole(int? id);

        Task<List<StudentDropdownModel>> GetMappedStudentByName(string StudentyName, int SectionId);
        Task<List<RoleModel>> InsertRole(RoleModel rolemaster);
        Task<List<RoleModel>> UpdateRole(RoleModel rolemaster);
        string DeleteRole(int id);
      
        Task<List<RoleActivity>> GetRoleActivity(int? id);
        Task<String> UpdateActivityFilepathdata(string target, int id,string  files);

        
        Task<List<FacultyModel>> GetFacultyDetails(int? Id);
        Task<List<FacultyModel>> InsertFacultyDetails(FacultyModel facultyDetails);
        Task<List<FacultyModel>> UpdateFacultyDetails(FacultyModel facultyDetails);
        Task<List<FacultyModel>> DeleteFacultyDetails(int Id);
        //Task<List<UserModel>> GetUserDetails(string Username, string Password,string role);
     
        Task<List<SubjectModel>> GetAllSubject(int? Id);
        Task<List<SubjectModel>> InsertSubjectDetails(SubjectModel subject);
        Task<List<SubjectModel>> UpdateSubjectDetails(SubjectModel subject);
        string DeleteSubjectDetails(int id);
        Task<string> bulkuploadstudent(DataTable target);

        Task<string> bulkuploadfaculty(DataTable target);
        Task<string> bulkuploadsubject(DataTable target);
        Task<string> bulkuploadholidaycalendar(DataTable target);
        Task<string> bulkuploadtimetable(DataTable target);
        Task<List<AttendanceModel>> GetAllAttendance(DateTime? AttendanceDate, int sectionId, string Hoursday);
        string InsertAttendance(List<AttendanceModel> attendance);
        Task<List<AttendanceModel>> UpdateAttendance(AttendanceModel attendance);
       // Task<List<SectionStudentSubjectList>> GetBatchWiseStudentList();
        Task<int> PasswordReset(string userName, string password);

        //Task<List<string>> GetSection(SectionModel sectionModel);
        //  Task<List<Sectiondetails>> GetSectionList(SectionListModel batchListModel);

        Task<List<BatchSubjectFacultyModel>> GetAllBatchSubMappings(int? id);
        Task<List<BatchSubjectFacultyModel>> InsertBatchSubMappings(BatchSubjectFacultyModel data);
        Task<List<BatchSubjectFacultyModel>> UpdateBatchSubMapping(BatchSubjectFacultyModel data);
        string DeleteBatchSubMapping(int id);
        //string generateAttendancereport();
        Task<List<BatchStudMappingModel>> GetAllSectionStudMappings(int? id);
        Task<int> InsertSectionStudMappings(List<BatchStudMappingModel> data);
        Task<int> UpdateSectionStudMapping(List<BatchStudMappingModel> model);
        Task<int> DeleteSectionStudMapping(int[] ids, int batchId);
        Task<List<SubjectAttendanceModel>> Getsubjectsforattendance(string batch, string Department, string Sem, string Year, string Section);
        public string generateMonthlyAttendancereport(int startMonth, int startYear, int endMonth, int endYear, int sectionId, string grade, string section);
        Task<(MemoryStream memory, string path)> DownloadData(string filepath);

      
        Task<List<StudentMark>> GetStudentMarkByIdDetails(int studentId);
        Task<List<StudentAttendanceModel>> GetAttendanceByIdDetails(int studentId, int month, int year);


        Task<List<AssignmentModel>> GetAllAssignmentDetails(int? id);
        Task<List<AssignmentModel>> InsertAssignmentDetails(AssignmentModel assignmentModel);
        Task<List<AssignmentModel>> UpdateAssignmentDetails(AssignmentModel assignmentModel);
        Task<List<AssignmentModel>> DeleteAssignmentDetails(int id);
        Task<List<AssignmentModel>> GetAllAssignmentByStudentDetails(string role,int studentId);

        Task<List<TimetableModel>> GetAllTimetableDetails(int? id);
        Task<List<TimetableModel>> GetTimeTableBySectionIdDetails(int sectionId,string role);
        Task<List<BatchSubjectFacultyModel>> GetFacultyListBySectionIdDetails(int sectionId);


        Task<List<TimetableModel>> InsertTimetableDetails(TimetableModel timetableModel);
        Task<List<TimetableModel>> UpdateTimetableDetails(TimetableModel timetableModel);
        Task<List<TimetableModel>> DeleteTimetableDetails(int id);

        Task<List<StudentFeedbackModel>> GetAllStudentFeedbackDetails(int? id,string role);
        Task<string> InsertStudentFeedbackDetails(List<StudentFeedbackModel> studentFeedbackModel);
        //Task<int> UpdateStudentFeedbackDetails(StudentFeedbackModel studentFeedbackModel);
        Task<List<StudentFeedbackModel>> DeleteStudentFeedbackDetails(string id);

        Task<List<UserFcmToken>> SaveFcmToken(UserFcmToken model);
        Task<List<UserFcmToken>> GetUserDeviceToken(int studentId, string role);
        Task<List<NotificationModel>> GetNotificationDetails(int studentId, string role);

        Task<List<NotificationModel>> UpdateNotificationDetails(NotificationModel notificationModel);
        Task<List<ContentLibModel>> GetAllContentLibDetails(int? id);
        Task<List<ContentLibModel>> InsertContentLibDetails(ContentLibModel contentLibModel);
        Task<List<ContentLibModel>> UpdateContentLibDetails(ContentLibModel contentLibModel);
        Task<List<ContentLibModel>> DeleteContentLibDetails(int id);
        Task<List<ContentLibModel>> GetAllContentLibByStudentDetails(int studentId);
        Task<List<BirthdayModel>> GetBirthdayListByRole(string role);

      
        Task<List<Feedbacksubject>> getSubFacultyList(int studentId);


        Task<List<AcademicCalender>> GetAllAcademicCalender(string role);
        Task<List<AcademicCalender>> InsertAcademicCalender(AcademicCalender academicCalender);
        Task<List<AcademicCalender>> UpdateAcademicCalender(AcademicCalender academicCalender);
        Task<List<AcademicCalender>> DeleteAcademicCalender(int Id);

        Task<List<InfoGaloreModel>> GetAllInfoGalore(string infoType, int? id);
        Task<List<InfoGaloreModel>> InsertInfoGalore(InfoGaloreModel infoGaloreModel);
        Task<List<InfoGaloreModel>> UpdateInfoGalore(int id,string target);

        Task<List<LeaveModel>> GetAllLeave(string role, int? id);
        Task<List<LeaveModel>> InsertLeave(LeaveModel model);
        Task<List<LeaveModel>> UpdateLeave(LeaveModel model);
        Task<List<LeaveModel>> DeleteLeave(int id);


        Task<List<HolidayCalendar>> GetHolidayCalendar(int? Id);
        Task<List<HolidayCalendar>> InsertHolidayCalendar(HolidayCalendar holiday);
        Task<List<HolidayCalendar>> UpdateHolidayCalendar(HolidayCalendar holiday);
        Task<List<HolidayCalendar>> DeleteHolidayCalendar(int Id);
    }
}
