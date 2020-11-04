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
        public string Message = "";
        private HttpClient client;

        public TaskFlyHelper(string token)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("api_key", token);
        }

        private async Task<T> CallService<T>(string httpVerb, string urlMethod, object data = null)
        {
            Message = "";
            HttpResponseMessage responsebody;
            var url = apiURL + urlMethod;
            HttpResponseMessage response = null;
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
                        response = await client.GetAsync(url);
                        break;
                    }
                case "POST":
                    {
                        response = await client.PostAsync(url, httpContent);
                        break;
                    }
                case "PUT":
                    {
                        response = await client.PutAsync(url, httpContent);
                        break;
                    }
                case "DELETE":
                    {
                        response = await client.DeleteAsync(url);
                        break;
                    }
                default:
                    {
                        response = null;
                        break;
                    }
            }
            responsebody = response;
            if(responsebody.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Message = responsebody.StatusCode.ToString();
            }
            var result = await responsebody.Content.ReadAsStringAsync();
            if (result == "")
            {
                Message = responsebody.StatusCode.ToString();
            }
            if (result.Contains(nameof(Message)))
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
                var newType = new { ID = 0 };
                if (httpVerb == "POST" && result != "")
                {
                    try
                    {
                        var obj = JsonConvert.DeserializeAnonymousType(result, newType);
                        if (Message == "") Message = "OK";
                        return JsonConvert.DeserializeObject<T>(obj.ID.ToString());
                    }
                    catch (Exception ex)
                    {
                        Message = ex.Message;
                        return JsonConvert.DeserializeObject<T>(newType.ID.ToString());
                    }
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(newType.ID.ToString());
                }
            }
        }

        public async Task<List<Boards>> GetBoards() => await CallService<List<Boards>>("GET", "/boards");
        public async Task<Boards> GetBoardsByID(int id) => await CallService<Boards>("GET", "/boards/" + id);
        public async Task<int> AddBoards(Boards board) => await CallService<int>("POST", "/boards", board);
        public async Task ChangeBoards(Boards board) => await CallService<object>("PUT", $"/boards/{board.Id}", board);
        public async Task<List<Customers>> GetCustomers() => await CallService<List<Customers>>("GET", "/customers");
        public async Task<Customers> GetCustomerByID(int id) => await CallService<Customers>("GET", "/customers/" + id);
        public async Task<int> GetCustomerByEmailDomain(string emailDomain) => await CallService<int>("POST", $"/customers/getbydomainname", new { domainname = emailDomain });
        public async Task<int> GetCustomerByDocument(string customerDocument) => await CallService<int>("POST", $"/customers/getbydocument", new { document = customerDocument });
        public async Task<int> AddCustomer(Customers customer) => await CallService<int>("POST","/customers",customer);
        public async Task ChangeCustomer(Customers customer) => await CallService<object>("PUT", $"/customers/{customer.Id}", customer);
        public async Task DeleteCustomer(int ID) => await CallService<object>("DELETE", $"/customers/{ID}", null);
        public async Task<List<Projects>> GetProjects() => await CallService<List<Projects>>("GET", "/projects");
        public async Task<int> AddProject(Projects project) => await CallService<int>("POST", "/projects", project);
        public async Task ChangeProject(Projects project) => await CallService<object>("PUT", $"/projects/{project.Id}", project);
        public async Task DeleteProject(int ID) => await CallService<object>("DELETE", $"/projects/{ID}", null);
        public async Task<List<Sectors>> GetSectors() => await CallService<List<Sectors>>("GET", "/sectors");
        public async Task<int> AddSector(Sectors sector) => await CallService<int>("POST", "/sectors", sector);
        public async Task ChangeSector(Sectors sector) => await CallService<object>("PUT", $"/sectors/{sector.Id}", sector);
        public async Task DeleteSector(int ID) => await CallService<object>("DELETE", $"/sectors/{ID}", null);
        public async Task<List<TaskPhases>> GetTaskPhases(int boardId) => await CallService<List<TaskPhases>>("GET", $"/taskphases/all/{boardId}");
        public async Task<int> AddPhase(TaskPhases taskPhase) => await CallService<int>("POST", "/taskphases", taskPhase);
        public async Task ChangePhase(TaskPhases taskPhase) => await CallService<object>("PUT", $"/taskphases/{taskPhase.Id}", taskPhase);
        public async Task DeletePhase(int ID) => await CallService<object>("DELETE", $"/taskphases/{ID}", null);
        public async Task<List<TaskPriority>> GetTaskPriority() => await CallService<List<TaskPriority>>("GET", "/taskpriorities");
        public async Task<int> AddPriority(TaskPriority taskPriority) => await CallService<int>("POST", "/taskpriorities", taskPriority);
        public async Task ChangePriority(TaskPriority taskPriority) => await CallService<object>("PUT", $"/taskpriorities/{taskPriority.Id}", taskPriority);
        public async Task DeletePriority(int ID) => await CallService<object>("DELETE", $"/taskpriorities/{ID}", null);
        public async Task<List<TaskType>> GetTaskType() => await CallService<List<TaskType>>("GET", "/tasktypes");
        public async Task<int> AddTaskType(TaskType taskType) => await CallService<int>("POST", "/tasktypes", taskType);
        public async Task ChangeTaskType(TaskType taskType) => await CallService<object>("PUT", $"/tasktypes/{taskType.Id}", taskType);
        public async Task DeleteTaskType(int ID) => await CallService<object>("DELETE", $"/tasktypes/{ID}", null);
        public async Task<List<Users>> GetUsers() => await CallService<List<Users>>("GET", "/users");
        public async Task<List<UsersToTransferTask>> GetUsersToTransferTask() => await CallService<List<UsersToTransferTask>>("GET", "/tasks/transfer/users");
        public async Task<List<TaskGetCustomFields>> GetTaskCustomFields() => await CallService<List<TaskGetCustomFields>>("GET", "/tasks/customfields");
        public async Task<List<Tasks>> GetTasks(Dictionary<string, object> filter)
        {
            var sb = new StringBuilder("?");
            foreach(var f in filter.ToList())
            {
                sb.Append($"{f.Key}={f.Value}&");
            }
            var strFilter = sb.ToString();
            strFilter = strFilter.Substring(0, strFilter.Length - 1);
            return await CallService<List<Tasks>>("GET", $"/tasks" + strFilter);
        }
        public async Task<List<Tasks>> GetTasksByTag(string taskTag, bool taskShowClosed = false) => await CallService<List<Tasks>>("POST", "/tasks/getbytag", new { tag = taskTag, showClosed = taskShowClosed });
        public async Task<int> AddTask(Tasks task) => await CallService<int>("POST", "/tasks", task);
        public async Task<List<TaskComments>> GetTaskComments(int ID) => await CallService<List<TaskComments>>("GET", $"/tasks/{ID}/comments");
        public async Task<List<UsersTasks>> GetUsersTaskCount() => await CallService<List<UsersTasks>>("GET", "/users/opentask");
        public async Task SendTaskComments(int ID, TaskComment comment) => await CallService<object>("POST", $"/tasks/{ID}/comments", comment);
        public async Task TaskStartTimer(int ID) => await CallService<object>("PUT", $"/tasks/{ID}/start", null);
        public async Task TaskStopTimer(int ID) => await CallService<object>("PUT", $"/tasks/{ID}/stop", null);
        public async Task TransferTask(int ID, int newUserId) => await CallService<object>("PUT",$"/tasks/{ID}/transfer/{newUserId}",null);
        public async Task SendTaskAttachmment(int ID, TaskAttachment attachment) => await CallService<object>("POST", $"/tasks/{ID}/attachments", attachment);
    }
}
