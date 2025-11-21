using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.Utilities
{
    public static class RetailCoreConstants
    {
        public static class Roles
        {
            public const string Role_Cust = "Customer";
            public const string Role_Comp = "Company";
            public const string Role_Admin = "Admin";
            public const string Role_Employee = "Employee";
        }

        public static class OrderStatus
        {
            public const string StatusPending = "Pending";
            public const string StatusApproved = "Approved";
            public const string StatusInProcess = "Processing";
            public const string StatusShipped = "Shipped";
            public const string StatusCancelled = "Cancelled";
            public const string StatusRefunded = "Refunded";

        }

        public static class PaymentStatus
        {
            public const string PaymentStatusPending = "Pending";
            public const string PaymentStatusApproved = "Approved";
            public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
            public const string PaymentStatusRejected = "Rejected";

        }

        public static class SessionConstants
        {
            public const string SessionCart = "SessionShoppingCart";
        }
    }
}
