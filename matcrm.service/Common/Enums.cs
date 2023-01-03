using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace matcrm.service.Common
{
    public class Enums
    {
        public enum PlaceType
        {
            InSideTheTextBoxes = 1,
            AboveTheTextBoxes = 2
        }

        public enum OrientationType
        {
            LeftSideTab = 1,
            RightSideTab = 2,
            LowerLeftTab = 3,
            LowerRightTab = 4,
            LightBox = 5,
        }

        public enum ImagePlacementType
        {
            Left = 2,
            Right = 3,
            Hidden = 1,
        }

        public enum Actions
        {
            NONE = 3
        }

        public enum ProjectActivityEnum
        {
            Project_Created = 1,
            Project_Updated = 2,
            Project_Removed = 3,
            Priority_changed_for_this_project = 4
        }

        public enum EmployeeTaskActivityEnum
        {
            Task_Created = 1,
            Task_Updated = 2,
            Task_Removed = 3,
            Priority_changed_for_this_task = 4,
            Task_assigned_to_user = 5,
            Time_record_added = 6,
            Task_comment_created = 7,
            Task_comment_updated = 8,
            Task_comment_removed = 9,
            Document_uploaded = 10,
            Document_removed = 11,
            Assign_user_removed = 12,
            Task_time_record_created = 13,
            Task_time_record_updated = 14,
            Task_time_record_removed = 15,
            Task_assign_to_Client = 16,
            Task_assign_to_project = 17,
            Unassign_client_from_task = 18,
            Unassign_project_from_task = 19,
        }

        public enum EmployeeProjectActivityEnum
        {
            Project_Created = 1,
            Project_Updated = 2,
            Project_Removed = 3,
            Priority_changed_for_this_Project = 4,
            Project_assigned_to_user = 5,
            Assign_user_removed = 6,
            Project_time_record_created = 7,
            Project_time_record_updated = 8,
            Project_time_record_removed = 9
        }
        public enum ContractActivityEnum
        {
            Contract_created = 1,
            Contract_updated = 2,
            Contract_removed = 3,
            Contract_article_created = 4,
            Contract_article_updated = 5,
            Contract_article_removed = 6,
            Contract_invoice_removed = 7
        }

        public enum EmployeeSubTaskActivityEnum
        {
            Subtask_time_record_created = 1,
            Subtask_time_record_updated = 2,
            Subtask_time_record_removed = 3,
            Subtask_comment_created = 4,
            Subtask_comment_removed = 5,
            Subtask_Created = 6,
            Subtask_Updated = 7,
            Subtask_assigned_to_user = 8,
            Subtask_Removed = 9
        }
        public enum EmployeeChidTaskActivityEnum
        {
            Childtask_time_record_created = 1,
            Childtask_time_record_updated = 2,
            Childtask_time_record_removed = 3,
            Childtask_comment_created = 4,
            Childtask_comment_removed = 5,
            Childtask_Created = 6,
            Childtask_Updated = 7,
            Childtask_assigned_to_user = 8,
            Childtask_Removed = 9
        }

        public enum ProjectContractActivityEnum
        {
            Project_added_into_contract = 1,
            //Project_updated_into_contract = 2,
            Project_removed_from_contract = 2
        }
        public enum MateTicketActivityEnum
        {
            Ticket_Created = 1,
            Ticket_Updated = 2,
            Ticket_assigned_to_user = 3,
            Ticket_assign_to_Client = 4,
            Ticket_assign_to_project = 5,
            Task_added_in_to_ticket = 6,
            Task_updated_in_to_ticket = 7,
            Ticket_comment_created = 8,
            Ticket_comment_removed = 9,
            Ticket_time_record_created = 10,
            Ticket_time_record_updated = 11,
            Ticket_time_record_removed = 12,
            Unassign_project_from_ticket = 13,
            Unassign_client_from_ticket = 14,
            Task_removed_from_ticket = 15,
            User_unassigned_from_ticket = 16,
            Ticket_removed = 17,
        }

        public enum WeekDays
        {
            SUNDAY = 1,
            MONDAY = 2,
            TUESDAY = 3,
            WEDNESDAY = 4,
            THURSDAY = 5,
            FRIDAY = 6,
            SATURDAY = 7
        }
    }
}