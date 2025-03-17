using System;
namespace ActivityManagementSystem.DAL.Constants
{
    public static class ConstantSPnames
	{
        public const string SP_GETALLSECTION = "sp_GetSectionMaster";
        public const string SP_INSERTSECTION = "sp_InsertSectionMaster";
        public static string SP_UPDATESECTION = "sp_UpdateSectionMaster";
        public static string SP_DELETESECTION = "sp_DeleteSectionMaster";

        public const string SP_GETSTUDENT = "sp_GetStudentDetails";
        public const string SP_INSERTSTUDENT = "sp_InsertStudentDetails";
        public const string SP_UPDATESTUDENT = "sp_UpdateStudentDetails";
        public const string SP_DELETESTUDENT = "sp_DeleteStudentDetails";

        public const string SP_GETALLHOUSE = "sp_GetHouseDetails";
        public const string SP_INSERTHOUSE = "sp_InsertHouseDetails";
        public const string SP_UPDATEHOUSE = "sp_UpdateHouseDetails";
        public const string SP_DELETEHOUSE = "sp_DeleteHouseDetails";

        public const string SP_GETHOUSEPOINT = "sp_GetHousepointbyHouseId";

        public const string SP_GETHOUSEACTIVITY = "[sp_GetHouseActivitypoint]";
        public const string SP_INSERTHOUSEACTIVITY = "InsertHousePointDetail";
        public const string SP_UPDATEHOUSEACTIVITY = "sp_UpdateHousePointDetail";
        public const string SP_DELETEHOUSEACTIVITY = "[sp_DeleteHousePointDetail]";

        public const string SP_INTERESTEDSTUDENTLIST = "[sp_GetInterestStudentListForCompetition]";
        public const string SP_UPDATEINTERESTED = "[sp_UpdateInterestedCompetition]";
        

        public const string SP_BULKSTUDENTUPLOAD = "[sp_BulkUploadStudent]";
        public const string SP_BULKFACULTYUPLOAD = "[sp_BulkUploadFaculty]";
        public const string SP_BULKSUBJECTUPLOAD = "[sp_BulkUploadSubject]";

        public const string SP_GETACTIVITIES = "sp_GetActivity";
		public const string SP_GETACTIVITYDETAILS= "sp_GetActivityData";
		public const string SP_GETAllACTIVITYDETAILS = "sp_GetAllActivityData";
		public const string SP_INSERTACTIVITYDETAILS = "sp_InsertActivityData";
		public const string SP_UPDATEACTIVITYDETAILS = "sp_UpdateActivityData";
		public const string SP_DELETEACTIVITYDETAILS = "sp_DeleteActivityData";
		public const string SP_GETALUMNIDETAILS = "sp_GetAlumniDetails";
		public const string SP_INSERTALUMNIDETAILS = "sp_InsertAlumniDetails";
		public const string SP_UPDATEALUMNIDETAILS = "sp_UpdateAlumniDetails";
		public const string SP_DELETEALUMNIDETAILS = "sp_DeleteAlumniDetails";
		public const string SP_GETROLEMASTER = "sp_GetRoleMaster";
		public const string SP_GETROLEACTIVITY= "sp_GetRoleActivity";
		public const string SP_INSERTROLEMASTER = "sp_InsertRoleMaster";
		public const string SP_UPDATEROLEMASTER = "sp_UpdateRoleMaster";
		public const string SP_UPDATEROLEACTIVITY = "sp_UpdateRoleActivity";
		public const string SP_INSERTROLEACTIVITY = "sp_InsertRoleActivityMappings";
		public const string SP_DELETEROLEMASTER = "sp_DeleteRoleMaster";
		public const string SP_UPDATEACTIVITYFILE = "sp_UpdateActivityFile";
		public const string SP_UPDATEFILE = "sp_UpdateFile";




		public const string SP_UPDATEACTIVITIES = "sp_UpdateActivity";
		
		public const string SP_GETFACULTY = "sp_GetFacultyDetailsById";
		public const string SP_INSERTFACULTY = "sp_InsertFacultyDetails";
		public const string SP_UPDATEFACULTY = "sp_UpdateFacultyDetails";
		public const string SP_DELETEFACULTY = "sp_DeleteFacultyDetails";
		public const string SP_GETUSERDETAILS= "sp_GetUserDetails";
		public const string SP_GETFACULTYBYNAME = "getFacultydetailsbyName";
        public static string SP_GETMAPPEDSTUDENTBYNAME = "[getMappedStudentbyName]";

        public const string SP_GETAlumniBYNAME = "getAlumnidetailsbyName";
		
		public const string SP_GETALLSUBJECT = "sp_GetAllSubject";
		public const string SP_INSERTSUBJECT = "sp_InsertSubject";
		public const string SP_UPDATESUBJECT = "sp_UpdateSubject";
		public const string SP_DELETESUBJECT = "sp_DeleteSubject";
		
		public const string SP_INSERTSECTIONSTUDMAP = "[sp_InsertSectionStudsMappings]";
		public const string SP_GETALLSECTIONSTUDMAP = "[sp_GetSectionStudsMappings]";
		public static string SP_UPDATESECTIONSTUDMAP = "sp_UpdateSectionStudsMappings";
		public static string SP_DELETESECTIONSTUDMAP = "[sp_DeleteSectionStudsMappings]";
		public static string SP_UPDATESECTIONSTUDACTIVEMAP = "[sp_UpdateActiveBatchStudMapping]";

		public const string TBL_STUDENT = "dbo.tbl_StudentDetails";
		public const string TBL_FACULTY = "dbo.tbl_FacultyDetails";
		public const string TBL_ALUMNI = "dbo.tbl_AlumniDetails";

		public static string SP_GETALLATTENDANCE = "sp_GetAttendanceNew";
		public static string SP_INSERTATTENDANCE = "sp_InsertAttendance";
		public static string SP_UPDATEATTENDANCE = "sp_UpdateAttendance";
		public static string SP_PASSWORDRESET = "sp_PasswordReset";
		public static string SP_DELETEATTENDANCE = "sp_DeleteAttendance1";


		public static string SP_GETSECTION = "sp_GetSection";
		public static string SP_GETBATCHLIST = "sp_GetBatchList";

		public const string SP_INSERTBATCHSUBMAP = "sp_InsertSectionSubMappings";
		public const string SP_GETALLBATCHSUBMAP = "sp_GetSectionSubFacultyMappings";
		public static string SP_UPDATEBATCHSUBMAP = "sp_UpdateSectionSubMappings";
		public static string SP_DELETEBATCHSUBMAP = "sp_DeleteBatchSubsMappings";

		public static string SP_UPDATEBATCHSUBACTIVEMAP = "sp_UpdateActiveBatchSubMapping";
		public const string SP_GETALLStudsListFromAdmission = "sp_GetStudentAdmissionList";
		public static string SP_UPDATESTUDDEPT = "sp_UpdateDeptIdAdmission";
		public static string SP_UPDATEDEPTNULL = "sp_UpdateDeptStudMapping";

		public static string  SP_Getsubjectsforattendance = "getsubjectsforattendance";


		public static string SP_GetDepartmentName = "sp_GetDepartmentName";
		public static string SP_MonthwiseAttendance = "GetStudentCumulativeAttendanceReport";
		public static string SP_MonthwiseDynamicAttendance = "sp_GetMonthAttendanceByMONTH";

		public const string TBL_SUBJECT = "dbo.tbl_Subject";

		public const string SP_GETALLSTUDENTDETAILWITHSECTION = "[Sp_GetAllStudentDetailWithSection]";


		

		public static string SP_GETVERIFYPASSWORD = "sp_GetVerifyPassword";
		public static string SP_UPDATEVERIFYPASSWORD = "sp_UpdateVerifyPassword";

        public static string SP_INSERTINDENT = "sp_InsertIndentForm";
        public static string SP_GETAllINDENTDETAILS = "sp_GetAllIndent";
        public static string SP_UPDATEINDENTDETAILS = "sp_UpdateIndentForm";
		public static string SP_UPDATEQUATATIONSTATUS = "sp_UpdateQuatationStatus";

		
		public static string SP_DELETEINDENTFORM = "sp_DeleteIndentForm";

        public static string SP_GETEXAMS = "[sp_GetExamMaster]";
        public static string SP_INSERTEXAMS = "[sp_InsertExamMaster]";
        public static string SP_UPDATEEXAMS = "[sp_UpdateExamMaster]";
        public static string SP_DELETEEXAMS = "[sp_DeleteExamMaster]";


		public static string SP_INSERTFDP = "sp_InsertFdpForm";
		public static string SP_GETAllFDPDETAILS = "sp_GetAllFdp";
		public static string SP_UPDATEFDPDETAILS = "sp_UpdateFdpForm";
		public static string SP_DELETEFDPFORM = "sp_DeleteFdpForm";


		public static string SP_GETINVENTORY = "[sp_GetInventorys]";
		public static string SP_INSERTINVENTORY = "[sp_InsertInventorys]";
		public static string SP_UPDATEINVENTORY = "[sp_UpdateInventorys]";
		public static string SP_DELETEINVENTORY = "[sp_DeleteInventorys]";



		public static string SP_GETINVENTORYSPEC = "[sp_GetInventorySpec]";
		public static string SP_INSERTINVENTORYSPEC = "[sp_InsertInventorySpec]";
		public static string SP_UPDATEINVENTORYSPEC = "[sp_UpdateInventorySpec]";
		public static string SP_DELETEINVENTORYSPEC = "[sp_DeleteInventorySpec]";



		public static string SP_GETSTOCKINVENTORY = "[sp_GetStockInventory]";
		public static string SP_INSERTSTOCKINVENTORY = "[sp_InsertStockInventory]";
		public static string SP_UPDATESTOCKINVENTORY = "[sp_UpdateStockInventory]";
		public static string SP_DELETESTOCKINVENTORY = "[sp_DeleteStockInventory]";



		public static string SP_INSERTINVENTORYISSUEDDETAILS = "sp_InsertInventoryIssuedDetails";
		public static string SP_GETALLINVENTORYISSUEDDETAILS = "sp_GetAllInventoryIssuedDetails";
		public static string SP_UPDATEINVENTORYISSUEDDETAILS = "sp_UpdateInventoryIssuedDetails";
		public static string SP_DELETEINVENTORYISSUED = "sp_DeleteAllInventoryIssuedDetails";
		public static string SP_STOCKREPORT = "[sp_StockReport]";

		public static string SP_GETALLLABDETAILS = "[sp_GetAllLabDetails]";
		public static string SP_INSERTLABDETAILS = "[sp_InsertLabDetails]";
		public static string SP_UPDATELABDETAILS = "[sp_UpdateLabDetails]";
		public static string SP_DELETELABDETAILS = "[sp_DeleteLabDetails]";



		public static string SP_GETALLHOADETAILS = "[sp_GetAllHOADetails]";
		public static string SP_INSERTHOADETAILS = "[sp_InsertHOADetails]";
		public static string SP_UPDATEHOADETAILS = "[sp_UpdateHOADetails]";
		public static string SP_DELETEHOADETAILS = "[sp_DeleteHOADetails]";



		public static string SP_GETPURCHASEDORDER = "[sp_GetPurchasedOrder]";
		public static string SP_INSERTPURCHASEDORDER = "[sp_InsertPurchasedOrder]";
		public static string SP_UPDATEPURCHASEDORDER = "[sp_UpdatePurchasedOrder]";
		public static string SP_DELETEPURCHASEDORDER = "[sp_DeletePurchasedOrder]";


		public static string SP_INSERTBUDGETLINE = "sp_InsertBudgetLine";
		public static string SP_GETALLBUDGETLINE = "sp_GetAllBudgetLine";
		public static string SP_UPDATEBUDGETLINE = "sp_UpdateBudgetLine";
		public static string SP_DELETEBUDGETLINE = "sp_DeleteBudgetLine";

		public static string SP_GETALLBUDGETHEAD = "sp_GetAllBudgetHead";
		public static string SP_INSERTBUDGETHEAD = "sp_InsertBudgetHead";
     	public static string SP_UPDATEBUDGETHEAD = "sp_UpdateBudgetHead";
		public static string SP_DELETEBUDGETHEAD = "sp_DeleteBudgetHead";


		public static string SP_REALLOCATEBUDGET="[sp_ReallocateBudgetDetails]";
		public static string SP_STOCKREPORTCUM = "[sp_StockReportCum]";

		public static string SP_INSERTUPCOMINGCOMPETITION = "[sp_InsertLeaveRequests]";
		public static string SP_GETALLUPCOMINGCOMPETITION = "[sp_GetLeaveRequests]";

        public static string SP_UPDATEUPCOMINGCOMPETITION = "[sp_UpdateLeaveRequests]";
		public static string SP_DELETEUPCOMINGCOMPETITION = "[sp_DeleteLeaveRequests]";

        public static string SP_INSERTLEAVE = "sp_InsertLEAVE";
        public static string SP_GETALLLEAVE = "sp_GetLEAVEs";
        public static string SP_UPDATELEAVE = "sp_UpdateLEAVE";
        public static string SP_DELETELEAVE = "sp_DeleteUpcomingCompetition";


        public static string SP_INSERTBPE = "[sp_InsertBpeForm]";
		public static string SP_GETAllBPEDETAILS = "sp_GetAllBpe";
		public static string SP_UPDATEBPEDETAILS = "sp_UpdateBpeForm";
		public static string SP_DELETEBPEFORM = "sp_DeleteBprForm";

		public static string SP_MARKTEMPLATE = "[sp_GetMarkTemplate]";
		public static string SP_Getsubjectsformarks = "[getsubjectsformarks]";


		public static string SP_GETSTUDENTMARKS = "[sp_getStudentMarks]";
		public static string SP_INSERTSTUDENTMARKS = "[sp_insertStudentMarks]";
		public static string TBL_STUDENTMARKS = "tbl_studentMarks";

		public const string SP_INSERTFACULTYSUBMAP = "sp_InsertFacultySubMappings";
		public const string SP_GETALLFACULTYSUBMAP = "sp_GetFacultySubsMapping";
		public static string SP_UPDATEFACULTYSUBMAP = "sp_UpdateFacultySubMapping";
		public static string SP_DELETEFACULTYSUBMAP = "sp_DeleteFacultySubsMappings"; 
		public static string SP_UPDATEFACULTYSUBACTIVEMAP = "sp_UpdateActiveFacultySubMapping";

		public static string SP_UPDATEEMAIL = "[UpdateSendEmail]";
		public static string SP_INSERTANNOUNCEMENTDETAILS = "sp_InsertAnnoucement";
		public static string SP_GETALLANNOUNCEMENTDETAILS = "[sp_GetAnnoucement]";
		public static string SP_DELETEANNOUNCEMENTDETAILS = "[ sp_DeleteAnnouncementDetails]";

		public static string SP_UPDATEATTPERCENTAGE = "[sp_UpdateAttPercentage]";
		public static string SP_GETFEEDBACKQNSREPORT = "[sp_GetFeedbackQns]";

		public static string SP_GETFEEDBACKFACULTYREPORT = "[sp_GetFeedbackFacultyReport]";

		public static string SP_UPDATESTUDENTSEMDATE = "[Sp_UpdateStudentSemDate]";
		public static string SP_DELETEMARK = "[sp_DeleteMark]";
		public static string SP_INSERTFEEDBACKFACULTYPERCENTAGE = "[sp_InsertFeedbackFacultyPercentage]";
		public static string SP_GETFEEDBACKFACULTYPERCENTAGE = "[sp_GetFeedbackFacultyPercentage]";

		public static string SP_GETFEEDBACKFACULTYPERCENTAGEREPORT = "[sp_GetFeedbackFacultyPercentageReport]";
		

		public static string SP_UPDATEFEEDBACK = "[UpdateIsFeedbackSend]";

		public static string SP_GETSTDCONGIG = "sp_GetAllStudentConfig";

		public static string SP_GETMARKSMSREPORTS = "sp_GetMarkSmsReports";

		public static string SP_GETSECWISEFEEDBACKREPORTS = "sp_GetSecWiseFeedbackReports";
		public static string getFacultySubjectforfeedback = "[getFacultySubjectforfeedback]";

		public static string SP_GETFEEDBACKEPORTSECTIONWISEWITHSUBREPORT = "[sp_GetFeedbackeportSectionwisewithSubReport]";
		public static string SP_GETSTUDENTASSREPORTDATA = "[sp_getStudentAssReportData]";

		public static string SP_GETMEMBERS = "sp_GetAllMemberDetails";
		public static string SP_INSERTMEMBERS = "sp_InsertMemberDetails";
		public static string SP_UPDATEMEMBERS = "sp_UpdateMemberDetails";
		public static string SP_DELETEMEMBERS = "sp_DeleteMemberDetails ";
		public static string TBL_MEMBER = "tbl_OtherMemberDetails";

		public static string SP_GETINDENTFACULTYMBLNUMBER = "sp_GetIndentFacultyMblNumber";
		public static string SP_GETFDPFACULTYMBLNUMBER = "sp_GetFdpFacultyMblNumber";
		public const string SP_UPDATEANNOUNCEMENTFILE = "SP_UPDATEANNOUNCEMENTFILE";

        public const string SP_GETSTUDENTMARKBYID = "SP_GetStudentMarkById";
        public const string SP_GETATTENDANCEBYID = "sp_GetMonthAttendanceByStudentId";

        public const string SP_GETALLASSIGNMENTDETAILS = "sp_GetAllAssignment";
		public const string SP_INSERTASSIGNMENTDETAILS = "[sp_InsertAssignment]";
        public const string SP_UPDATEASSIGNMENTDETAILS = "[sp_UpdateAssignment]";
        public const string SP_DELETEASSIGNMENTFORM = "[sp_DeleteAssignment]";
        public const string SP_GETALLASSIGNMENTBYSTUDENT = "sp_GetAllAssignmentByStudent"; 


        public const string SP_GETALLTIMETABLEDETAILS = "sp_GetAllTimetable";
        public const string SP_GetTimeTableBySectionIdDETAILS = "sp_GetTimeTableBySectionId";
        public const string SP_GetFacultyListBySectionIdDETAILS = "sp_GetFacultyListBySectionId";

		public const string SP_GETALLFACULTYLIST = "[sp_GetAllFacultyList]";
        public const string SP_INSERTTIMETABLEDETAILS = "[sp_InsertTimetable]";
        public const string SP_UPDATETIMETABLEDETAILS = "[sp_UpdateTimetable]";
        public const string SP_DELETETIMETABLE = "[sp_DeleteTimetable]";


        public const string SP_GETALLSTUDENTFEEDBACKDETAILS = "sp_GetAllStudentFeedback";
        public const string SP_INSERTSTUDENTFEEDBACKDETAILS = "[sp_InsertStudentFeedback]";
        public const string SP_UPDATESTUDENTFEEDBACKDETAILS = "[sp_UpdateStudentFeedback]";
        public const string SP_DELETESTUDENTFEEDBACK = "[sp_DeleteStudentFeedback]";

		public const string SP_GETNOTIFICATION = "sp_GetNotification";
        public const string SP_UPDATENOTIFICATION = "[sp_UpdateNotification]";
        public const string SP_GETUSERTOKEN = "sp_GetUserToken";
		public const string SP_INSERTUSERFCMTOKENS = "sp_InsertUserFCMTokens";



        public const string SP_GETCONTENTLIBDETAILS = "sp_GetContentLib";
        public const string SP_INSERTCONTENTLIBDETAILS = "[sp_InsertContentLib]";
        public const string SP_UPDATECONTENTLIBDETAILS = "[sp_UpdateContentLib]";
        public const string SP_DELETECONTENTLIB = "[sp_DeleteContentLib]";
        public const string SP_GETALLCONTENTLIBBYSTUDENT = "sp_GetAllContentLibByStudent";

        public const string SP_GETALLBIRTHDAYLIST = "sp_GetBirthdayListByRole";


        public const string SP_GETACADEMICCALENDER = "sp_GetAcademicCalender";
        public const string SP_INSERTACADEMICCALENDER = "sp_InsertAcademicCalender";
        public const string SP_UPDATEACADEMICCALENDER = "sp_UpdateAcademicCalender";
        public const string SP_DELETEACADEMICCALENDER = "sp_DeleteAcademicCalender";

        public const string SP_GETINFOGALORE = "sp_GetAllInfoGalore";
		public const string SP_INSERTINFOGALORE = "sp_InsertInfoGalore";
		public const string SP_UPDATEINFOGALORE = "sp_UpdateInfoGalore";

    }
}
