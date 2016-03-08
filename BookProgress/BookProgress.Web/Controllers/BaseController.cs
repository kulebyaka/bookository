using GdfSuezGenesis.Service.Logger;
using System.Collections.Generic;
using System.Web.Mvc;
using GdfSuezGenesis.BBInterface;
using GdfSuezGenesis.BBInterface.Logging;
using System.Configuration;
using System.Web;
using GdfSuezGenesis.BBInterface.Entities;
using GdfSuezGenesis.BBInterface.Service;
using GdfSuezGenesis.BBInterface.Service.Customer;
using System.Linq;

namespace GdfSuezGenesis.Controllers
{
    public class BaseController : Controller
    {
        private List<SentMessage> _messages = new List<SentMessage>();

        private readonly BBServiceFactory _bbServiceFactory = null;

        private CustomerService _customerService;

        private AccountService _accountService;

        public BaseController()
        {
            _bbServiceFactory = new BBServiceFactory(ConfigurationManager.AppSettings["BackBoneURI"]);
            _bbServiceFactory.AddEndpointBehavior(new LoggingEndpointBehavior(new HttpContextMessageLogger()));

            _customerService = new CustomerService(_bbServiceFactory);
            _accountService = new AccountService(_bbServiceFactory);
        }

        public BBServiceFactory BB
        {
            get
            {
                return _bbServiceFactory;
            }
        }

        public CustomerService CustomerService
        {
            get
            {
                return _customerService;
            }
        }

        public AccountService AccountService
        {
            get
            {
                return _accountService;
            }
        }

        public static bool Debug
        {
            get
            {
                return ConfigurationManager.AppSettings["Debug"] == "true";
            }
        }

        protected bool IsFormSubmit()
        {
            return Request.Params["modalFormSubmit"] != null;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public bool IsError<T>(IServiceResponse<T> response) where T : BaseEntity
        {
            return response.Header.GlobalMessage.Status == ResponseHeaderMessageStatus.Error;
        }

        public bool IsError<T>(ServiceResult<T> result) where T : BaseEntity
        {
            return result.IsError;
        }

        public static class StatusMessages
        {
            public static void Add(IEnumerable<StatusMessage> messages)
            {
                List<StatusMessage> _messages;
                if (System.Web.HttpContext.Current.Session["statusMessages"] == null)
                {
                    _messages = new List<StatusMessage>();
                    System.Web.HttpContext.Current.Session["statusMessages"] = _messages;
                }
                else
                {
                    _messages = (List<StatusMessage>)System.Web.HttpContext.Current.Session["statusMessages"];
                }
                _messages.AddRange(messages);
            }

            public static void Add(string message, ResponseHeaderMessageStatus status)
            {
                Add(new StatusMessage { Status = status, Message = message });
            }

            public static void Add(StatusMessage statusMessage)
            {
                Add(new List<StatusMessage>() { statusMessage });
            }


            public static void Add<T>(ServiceResult<T> result) where T : BaseEntity
            {
                Add(result.StatusMessages);
            }

            public static void AddWithSuccessText<T>(ServiceResult<T> result, string successMessage) where T : BaseEntity
            {
                if (result.IsSuccess)
                {
                    // Add custom success message:
                    Add(successMessage, ResponseHeaderMessageStatus.Info);
                    // Add warnings, if any:
                    Add(result.StatusMessages.Where(msg => msg.IsWarning));
                }
                else
                {
                    Add(result.StatusMessages);
                }
            }

            public static void Add<T>(IServiceResponse<T> response) where T : BaseEntity
            {
                var globalMessage = response.Header.GlobalMessage;
                Add(globalMessage.Message, globalMessage.Status);
            }

            public static void AddIfNotOk<T>(IServiceResponse<T> response) where T : BaseEntity
            {
                var globalMessage = response.Header.GlobalMessage;
                if (globalMessage.Status != ResponseHeaderMessageStatus.Info)
                {
                    Add(globalMessage.Message, globalMessage.Status);
                }
            }
        }
    }
    
}