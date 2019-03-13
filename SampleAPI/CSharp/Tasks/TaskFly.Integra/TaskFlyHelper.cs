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

        private T CallService<T>(string method)
        {
            Message = "";
            using (var client = GetHttpClientToken())
            {
                var url = apiURL + method;
                using (var response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();
                    var responsebody = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(responsebody);
                }
            }
        }

        private int SendToService(string httpVerb, string urlMethod, object data)
        {
            int ID = 0;
            Message = "";
            using (var client = GetHttpClientToken())
            {
                HttpResponseMessage responsebody;
                var url = apiURL + urlMethod;
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
                Task<HttpResponseMessage> response = null;
                switch (httpVerb)
                {
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
                if(result.Contains("Message"))
                {
                    var newType = new { Message = "" };
                    var obj = JsonConvert.DeserializeAnonymousType(result, newType);
                    Message = obj.Message;
                }
                if (httpVerb == "POST" && result != "")
                {
                    var newType = new { ID = 0 };
                    var obj = JsonConvert.DeserializeAnonymousType(result, newType);
                    ID = obj.ID;
                    if(Message == "") Message = "OK";
                }
            }
            return ID;
        }

        public List<Customers> GetCustomers() => CallService<List<Customers>>("/customers");
        public Customers GetCustomerByID(int id) => CallService<Customers>("/customers/" + id);
        public int GetCustomerByEmailDomain(string emailDomain) => SendToService("POST",$"/customers/getbydomainname", new { domainname = emailDomain });
        public int AddCustomer(Customers customer) => SendToService("POST","/customers",customer);
        public void ChangeCustomer(Customers customer) => SendToService("PUT", $"/customers/{customer.Id}", customer);
        public void DeleteCustomer(int ID) => SendToService("DELETE", $"/customers/{ID}", null);
        public List<Projects> GetProjects() => CallService<List<Projects>>("/projects");
        public int AddProject(Projects project) => SendToService("POST", "/projects", project);
        public void ChangeProject(Projects project) => SendToService("PUT", $"/projects/{project.Id}", project);
        public void DeleteProject(int ID) => SendToService("DELETE", $"/projects/{ID}", null);
        public List<Sectors> GetSectors() => CallService<List<Sectors>>("/sectors");
        public int AddSector(Sectors sector) => SendToService("POST", "/sectors", sector);
        public void ChangeSector(Sectors sector) => SendToService("PUT", $"/sectors/{sector.Id}", sector);
        public void DeleteSector(int ID) => SendToService("DELETE", $"/sectors/{ID}", null);
        public List<TaskPhases> GetTaskPhases() => CallService<List<TaskPhases>>("/taskphases");
        public int AddPhase(TaskPhases taskPhase) => SendToService("POST", "/taskphases", taskPhase);
        public void ChangePhase(TaskPhases taskPhase) => SendToService("PUT", $"/taskphases/{taskPhase.Id}", taskPhase);
        public void DeletePhase(int ID) => SendToService("DELETE", $"/taskphases/{ID}", null);
        public List<TaskPriority> GetTaskPriority() => CallService<List<TaskPriority>>("/taskpriorities");
        public int AddPriority(TaskPriority taskPriority) => SendToService("POST", "/taskpriorities", taskPriority);
        public void ChangePriority(TaskPriority taskPriority) => SendToService("PUT", $"/taskpriorities/{taskPriority.Id}", taskPriority);
        public void DeletePriority(int ID) => SendToService("DELETE", $"/taskpriorities/{ID}", null);
        public List<TaskType> GetTaskType() => CallService<List<TaskType>>("/tasktypes");
        public int AddTaskType(TaskType taskType) => SendToService("POST", "/tasktypes", taskType);
        public void ChangeTaskType(TaskType taskType) => SendToService("PUT", $"/tasktypes/{taskType.Id}", taskType);
        public void DeleteTaskType(int ID) => SendToService("DELETE", $"/tasktypes/{ID}", null);
        public List<Users> GetUsers() => CallService<List<Users>>("/users");
        public List<UsersToTransferTask> GetUsersToTransferTask() => CallService<List<UsersToTransferTask>>("/tasks/transfer/users");
        public List<Tasks> GetTasks(Dictionary<string, object> filter)
        {
            var sb = new StringBuilder("?");
            foreach(var f in filter.ToList())
            {
                sb.Append($"{f.Key}={f.Value}&");
            }
            var strFilter = sb.ToString();
            strFilter = strFilter.Substring(0, strFilter.Length - 1);
            return CallService<List<Tasks>>($"/tasks" + strFilter);
        }
        public int AddTask(Tasks task) => SendToService("POST", "/tasks", task);
        public List<TaskComments> GetTaskComments(int ID) => CallService<List<TaskComments>>($"/tasks/{ID}/comments");
        public void SendTaskComments(int ID, TaskComment comment) => SendToService("POST", $"/tasks/{ID}/comments", comment);
        public void TaskStartTimer(int ID) => SendToService("PUT", $"/tasks/{ID}/start", null);
        public void TaskStopTimer(int ID) => SendToService("PUT", $"/tasks/{ID}/stop", null);
        public void TransferTask(int ID, int newUserId) => SendToService("PUT",$"/tasks/{ID}/transfer/{newUserId}",null);
        public void SendTaskAttachmment(int ID, TaskAttachment attachment) => SendToService("POST", $"/tasks/{ID}/attachments", attachment);
    }
}
