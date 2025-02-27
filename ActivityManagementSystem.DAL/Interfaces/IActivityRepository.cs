using ActivityManagementSystem.Domain.Models.Activity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.DAL.Interfaces
{
    public interface IActivityRepository : IDisposable
    {

        Task<List<StudentModel>> GetStudentDetails(int? Id);
        Task<List<StudentModel>> InsertStudentDetails(StudentModel studentDetails);
        Task<List<StudentModel>> UpdateStudentDetails(StudentModel studentDetails);
        Task<List<StudentModel>> DeleteStudentDetails(int Id);
        Task<List<SectionModel>> GetAllSectiones(int? Id);
        Task<List<SectionModel>> InsertSectionDetails(SectionModel sectionModel);
        Task<List<SectionModel>> UpdateSectionDetails(SectionModel sectionModel);
        Task<List<SectionModel>> DeleteSectionDetails(int Id);


        Task<List<ExamsModel>> GetExams(int? id);
        Task<List<ExamsModel>> InsertExams(ExamsModel roleModel);
        Task<List<ExamsModel>> UpdateExams(ExamsModel roleModel);
        Task<string> DeleteExams(int id);


        Task<List<ActivityModel>> GetAllActivityData(int Type, long? DepartmentId);
        Task<List<ActivityModel>> GetActivityData(int? id);
        Task<List<ActivityModel>> InsertActivityData(ActivityModel activitydetails);
        Task<List<ActivityModel>> UpdateActivityData(ActivityModel activitydetails);
        Task<List<ActivityModel>> DeleteActivityData(int id);
      
        Task<List<RoleModel>> GetRole(int? id);
        Task<List<RoleModel>> InsertRole(RoleModel rolemaster);
        Task<List<RoleModel>> UpdateRole(RoleModel rolemaster);
        string DeleteRole(int id);
        Task<List<RoleActivity>> UpdateRoleActivity(RoleActivity roleactivity);
        Task<List<RoleActivity>> GetRoleActivity(int? id);

        Task<List<UserModel>> GetUserDetails(string Username, string Password,string role);

        Task<List<FacultyDropdown>> GetFacultyByName(string facultyName);
        Task<List<StudentDropdown>> GetStudentByName(string StudentyName);
        Task<string> UpdateActivityFilepathdata(string target, int id, string files);
        
        Task<List<FacultyModel>> GetFacultyDetails(int? Id);
        string InsertFacultyDetails(FacultyModel facultyDetails);
        string UpdateFacultyDetails(FacultyModel facultyDetails);
        Task<List<FacultyModel>> DeleteFacultyDetails(int Id);
        Task<List<HouseModel>> GetAllHouse(int? Id);
        Task<List<HouseModel>> InsertHouseDetails(HouseModel house);
        Task<List<HouseModel>> UpdateHouseDetails(HouseModel house);
        Task<List<HouseModel>> DeleteHouseDetails(int id);

        Task<List<SubjectModel>> GetAllSubject(int? Id);
        Task<List<SubjectModel>> InsertSubjectDetails(SubjectModel subject);
        Task<List<SubjectModel>> UpdateSubjectDetails(SubjectModel subject);
        string DeleteSubjectDetails(int id);
        Task<string> bulkuploadstudent(DataTable target);

        Task<string> bulkuploadfaculty(DataTable target);
        Task<string> bulkuploadsubject(DataTable target);

        Task<List<AttendanceModel>> GetAllAttendance(DateTime? AttendanceDate, int sectionId, string Hoursday);
        string InsertAttendance(List<AttendanceModel> attendance);
        Task<List<AttendanceModel>> UpdateAttendance(AttendanceModel attendance);

     //   Task<List<BatchStudentSubjectList>> GetBatchWiseStudentList();
        Task<int> PasswordReset(string userName, string password);
        
     //   Task<List<string>> GetSection(SectionModel sectionModel);
        Task<List<Batchdetails>> GetBatchList(BatchListModel batchListModel);

        Task<List<BatchSubjectFacultyModel>> GetAllBatchSubMapping(int? id);
        Task<List<BatchSubjectFacultyModel>> InsertBatchSubMappings(BatchSubjectFacultyModel data);
        Task<List<BatchSubjectFacultyModel>> UpdateBatchSubMapping(BatchSubjectFacultyModel data);
        string DeleteBatchSubMapping(int id);
        //Task<List<BatchStudMappingModel>> GetAllBatchStudMappings(int? id);
        Task<int> InsertSectionStudMappings(List<BatchStudMappingModel> data);
        Task<int> UpdateSectionStudMapping(List<BatchStudMappingModel> model);
        Task<int> DeleteSectionStudMapping(int[] ids, int batchId);


        Task<List<SubjectAttendanceModel>> Getsubjectsforattendance(string batch, string Department, string Sem, string Year, string Section);
        public string generateAttendancesubjectwisereport(string SubjectCode, string Sem, string Year, string DepartmentId);
        public string generateAttendancedynamicreport(string Sem, string Year, int Department, string Section);
        public string generateMonthlyAttendancereport(string Sem, string Year, int Department, DateTime AttendanceDate, string Batch);

        Task<List<MemberDetails>> DeleteMembersDetails(int id);
        Task<List<MemberDetails>> UpdateMembersDetails(MemberDetails memberDetails);
        Task<List<MemberDetails>> InsertMembersDetails(MemberDetails memberDetails);
        Task<List<MemberDetails>> GetMembersDetails(int? id);
        Task<List<StudentMark>> GetStudentMarkByIdDetails(int studentId);
        Task<List<StudentAttendanceModel>> GetAttendanceByIdDetails(int studentId, int month, int year);


        Task<List<AssignmentModel>> GetAllAssignmentDetails(int? id);
        Task<List<AssignmentModel>> InsertAssignmentDetails(AssignmentModel assignmentModel);
        Task<List<AssignmentModel>> UpdateAssignmentDetails(AssignmentModel assignmentModel);
        Task<List<AssignmentModel>> DeleteAssignmentDetails(int id);
        Task<List<AssignmentModel>> GetAllAssignmentByStudentDetails(int studentId);


        Task<List<TimetableModel>> GetAllTimetableDetails(int? id);
        Task<List<TimetableModel>> InsertTimetableDetails(TimetableModel timetableModel);
        Task<List<TimetableModel>> UpdateTimetableDetails(TimetableModel timetableModel);
        Task<List<TimetableModel>> DeleteTimetableDetails(int id);



        Task<List<StudentFeedbackModel>> GetAllStudentFeedbackDetails(int? id);
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

        Task<string> UpdateSubmitStatus(int studentId);
        Task<string> InsertFeedBackDetails(Feedback feedback);
        Task<List<QnsModel>> getQuestions();
        Task<Feedback> getFeedbackDetails(int studentId, int subjectId, int facultyId);
        Task<List<Feedbacksubject>> getSubFacultyList(int studentId);
        Task<List<Feedbacksubject>> checkFeedbackSubmittedAsync(int studentId);

        Task<List<AcademicCalender>> GetAllAcademicCalender(string role);
        Task<List<AcademicCalender>> InsertAcademicCalender(AcademicCalender academicCalender);
        Task<List<AcademicCalender>> UpdateAcademicCalender(AcademicCalender academicCalender);
        Task<List<AcademicCalender>> DeleteAcademicCalender(int SNo);

        Task<List<InfoGaloreModel>> GetAllInfoGalore(int? id);
        Task<List<InfoGaloreModel>> InsertInfoGalore(InfoGaloreModel infoGaloreModel);
        Task<List<InfoGaloreModel>> UpdateInfoGalore(int id, string target);
    }

}

