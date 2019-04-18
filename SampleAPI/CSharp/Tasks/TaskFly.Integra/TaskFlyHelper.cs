using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskFly.Entities;

namespace TaskFly.Integra
{
    public class TaskFlyHelper
    {
        const string apiURL = "https://integra.gotaskfly.com/api/v1";
        private readonly string apiToken;
        public string Message = "";

        public TaskFlyHelper(string token)
        {
            apiToken = token;
        }

        private HttpClient GetHttpClientToken()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("api_key", apiToken);
            return client;
        }

        private T CallService<T>(string httpVerb, string urlMethod, object data = null)
        {
            Message = "";
            using (var client = GetHttpClientToken())
            {
                HttpResponseMessage responsebody;
                var url = apiURL + urlMethod;
                Task<HttpResponseMessage> response = null;
                HttpContent httpContent;
                if (data != null)
                {
                    var content = JsonConvert.SerializeObject(data);
                    var buffer = Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    httpContent = byteContent;
                }
                else
                {
                    var content = new StringContent("");
                    content.Headers.ContentType = null;
                    httpContent = content;
                }

                switch (httpVerb)
                {
                    case "GET":
                        {
                            response = client.GetAsync(url);
                            break;
                        }
                    case "POST":
                        {
                            response = client.PostAsync(url, httpContent);
                            break;
                        }
                    case "PUT":
                        {
                            response = client.PutAsync(url, httpContent);
                            break;
                        }
                    case "DELETE":
                        {
                            response = client.DeleteAsync(url);
                            break;
                        }
                }
                responsebody = response.Result;
                string result = (responsebody.Content.ReadAsStringAsync().Result);
                if (result == "")
                {
                    Message = responsebody.StatusCode.ToString();
                }
                if (result.Contains("Message"))
                {
                    var newType = new { Message = "" };
                    var obj = JsonConvert.DeserializeAnonymousType(result, newType);
                    Message = obj.Message;
                }
                if (httpVerb == "GET")
                {
                    return JsonConvert.DeserializeObject<T>(result);
                }
                else
                {
                    if (httpVerb == "POST" && result != "")
                    {
                        try
                        {
                            var newType = new { ID = 0 };
                            var obj = JsonConvert.DeserializeAnonymousType(result, newType);
                            if (Message == "") Message = "OK";
                            return JsonConvert.DeserializeObject<T>(obj.ID.ToString());
                        }
                        catch (Exception ex)
                        {
                            Message = ex.Message;
                            return JsonConvert.DeserializeObject<T>("0");
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<T>("0");
                    }
                }
            }
        }

        public List<Customers> GetCustomers() => CallService<List<Customers>>("GET", "/customers");
        public Customers GetCustomerByID(int id) => CallService<Customers>("GET", "/customers/" + id);
        public int GetCustomerByEmailDomain(string emailDomain) => CallService<int>("POST", $"/customers/getbydomainname", new { domainname = emailDomain });
        public int GetCustomerByDocument(string customerDocument) => CallService<int>("POST", $"/customers/getbydocument", new { document = customerDocument });
        public int AddCustomer(Customers customer) => CallService<int>("POST","/customers",customer);
        public void ChangeCustomer(Customers customer) => CallService<object>("PUT", $"/customers/{customer.Id}", customer);
        public void DeleteCustomer(int ID) => CallService<object>("DELETE", $"/customers/{ID}", null);
        public List<Projects> GetProjects() => CallService<List<Projects>>("GET", "/projects");
        public int AddProject(Projects project) => CallService<int>("POST", "/projects", project);
        public void ChangeProject(Projects project) => CallService<object>("PUT", $"/projects/{project.Id}", project);
        public void DeleteProject(int ID) => CallService<object>("DELETE", $"/projects/{ID}", null);
        public List<Sectors> GetSectors() => CallService<List<Sectors>>("GET", "/sectors");
        public int AddSector(Sectors sector) => CallService<int>("POST", "/sectors", sector);
        public void ChangeSector(Sectors sector) => CallService<object>("PUT", $"/sectors/{sector.Id}", sector);
        public void DeleteSector(int ID) => CallService<object>("DELETE", $"/sectors/{ID}", null);
        public List<TaskPhases> GetTaskPhases() => CallService<List<TaskPhases>>("GET", "/taskphases");
        public int AddPhase(TaskPhases taskPhase) => CallService<int>("POST", "/taskphases", taskPhase);
        public void ChangePhase(TaskPhases taskPhase) => CallService<object>("PUT", $"/taskphases/{taskPhase.Id}", taskPhase);
        public void DeletePhase(int ID) => CallService<object>("DELETE", $"/taskphases/{ID}", null);
        public List<TaskPriority> GetTaskPriority() => CallService<List<TaskPriority>>("GET", "/taskpriorities");
        public int AddPriority(TaskPriority taskPriority) => CallService<int>("POST", "/taskpriorities", taskPriority);
        public void ChangePriority(TaskPriority taskPriority) => CallService<object>("PUT", $"/taskpriorities/{taskPriority.Id}", taskPriority);
        public void DeletePriority(int ID) => CallService<object>("DELETE", $"/taskpriorities/{ID}", null);
        public List<TaskType> GetTaskType() => CallService<List<TaskType>>("GET", "/tasktypes");
        public int AddTaskType(TaskType taskType) => CallService<int>("POST", "/tasktypes", taskType);
        public void ChangeTaskType(TaskType taskType) => CallService<object>("PUT", $"/tasktypes/{taskType.Id}", taskType);
        public void DeleteTaskType(int ID) => CallService<object>("DELETE", $"/tasktypes/{ID}", null);
        public List<Users> GetUsers() => CallService<List<Users>>("GET", "/users");
        public List<UsersToTransferTask> GetUsersToTransferTask() => CallService<List<UsersToTransferTask>>("GET", "/tasks/transfer/users");
        public List<Tasks> GetTasks(Dictionary<string, object> filter)
        {
            var sb = new StringBuilder("?");
            foreach(var f in filter.ToList())
            {
                sb.Append($"{f.Key}={f.Value}&");
            }
            var strFilter = sb.ToString();
            strFilter = strFilter.Substring(0, strFilter.Length - 1);
            return CallService<List<Tasks>>("GET", $"/tasks" + strFilter);
        }
        public List<Tasks> GetTasksByTag(string taskTag, bool taskShowClosed = false) => CallService<List<Tasks>>("POST", "/tasks/getbytag", new { tag = taskTag, showClosed = taskShowClosed });
        public int AddTask(Tasks task) => CallService<int>("POST", "/tasks", task);
        public List<TaskComments> GetTaskComments(int ID) => CallService<List<TaskComments>>("GET", $"/tasks/{ID}/comments");
        public List<UsersTasks> GetUsersTaskCount() => CallService<List<UsersTasks>>("GET", "/users/opentask");
        public void SendTaskComments(int ID, TaskComment comment) => CallService<object>("POST", $"/tasks/{ID}/comments", comment);
        public void TaskStartTimer(int ID) => CallService<object>("PUT", $"/tasks/{ID}/start", null);
        public void TaskStopTimer(int ID) => CallService<object>("PUT", $"/tasks/{ID}/stop", null);
        public void TransferTask(int ID, int newUserId) => CallService<object>("PUT",$"/tasks/{ID}/transfer/{newUserId}",null);
        public void SendTaskAttachmment(int ID, TaskAttachment attachment) => CallService<object>("POST", $"/tasks/{ID}/attachments", attachment);
    }
}
