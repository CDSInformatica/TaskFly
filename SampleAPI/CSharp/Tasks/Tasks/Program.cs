using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaskFly.Entities;
using TaskFly.Integra;

namespace TaskFlySampleAPI
{
    class Program
    {
        private static TaskFlyHelper taskfly;

        private static async Task Main()
        {
            // API V1 url: https://integra.gotaskfly.com/api/v1
            // API Documentation: https://integra.gotaskfly.com/docs/index
            // Token is generated on TaskFly/User Profile/Integration
            // Each Team has an integration token
            var APIToken = "<YOUR_USER_PROFILE_TOKEN>";         
            taskfly = new TaskFlyHelper(APIToken);

            //BoardsSample();
            //UsersSample();
            //CustomersSample();
            //ProjectsSample();
            //SectorsSample();
            //PhasesSample();
            //PrioritySample();
            //TaskTypeSample();
            await TaskSample();
        }

        private static async Task BoardsSample()
        {
            var boards = await taskfly.GetBoards();
        }

        private static async Task UsersSample()
        {
            var users = await taskfly.GetUsers();
            var userTask = await taskfly.GetUsersTaskCount();
        }

        private static async Task CustomersSample()
        {
            var customers = await taskfly.GetCustomers();
            var newCustomer = new Customers()
            {
                CompanyName = "Customers Sample",
                Name = "Customers Sample Name",
                Address = "Av. Alberto Carazzai, 762",
                City = "Cornelio Procopio",
                District = "Vila Ipiranga",
                UF = "PR",
                Contact = "Carlos dos Santos",
                Email = "cds@cds-software.com.br",
                Active = true
            };
            int newCustomerId = await taskfly.AddCustomer(newCustomer);
            newCustomer.Id = newCustomerId;
            newCustomer.Active = true;
            newCustomer.Name = "Customer Sample Changed";
            await taskfly.ChangeCustomer(newCustomer);
            await taskfly.DeleteCustomer(newCustomer.Id);
            var customer1 = await taskfly.GetCustomerByID(1);

            var customerID = await taskfly.GetCustomerByEmailDomain("cds-software.com.br");

            var customerIDDoc = await taskfly.GetCustomerByDocument("SOME_DOC_ID");
        }

        private static async Task ProjectsSample()
        {
            var projects = await taskfly.GetProjects();
            var newPrj = new Projects()
            {
                Description = "Project Inserted"
            };
            int newId = await taskfly.AddProject(newPrj);
            newPrj.Id = newId;
            newPrj.Description = "Project Changed";
            await taskfly.ChangeProject(newPrj);
            await taskfly.DeleteProject(newId);
        }

        private static async Task SectorsSample()
        {
            var sectors = await taskfly.GetSectors();
            var newSector = new Sectors()
            {
                Description = "Sector Inserted"
            };
            int newId = await taskfly.AddSector(newSector);
            newSector.Id = newId;
            newSector.Description = "Sector Changed";
            await taskfly.ChangeSector(newSector);
            await taskfly.DeleteSector(newId);
        }

        private static async Task PhasesSample()
        {
            var boards = await taskfly.GetBoards();
            var phases = await taskfly.GetTaskPhases(boards.First().Id);
            var newPhase = new TaskPhases()
            {
                Description = "Phase Inserted"
            };
            int newId = await taskfly.AddPhase(newPhase);
            newPhase.Id = newId;
            newPhase.Description = "Phase Changed";
            await taskfly.ChangePhase(newPhase);
            await taskfly.DeletePhase(newId);
        }

        private static async Task PrioritySample()
        {
            var priority = await taskfly.GetTaskPriority();
            var newPriority = new TaskPriority()
            {
                Description = "Priority Inserted"
            };
            int newId = await taskfly.AddPriority(newPriority);
            newPriority.Id = newId;
            newPriority.Description = "Priority Changed";
            await taskfly.ChangePriority(newPriority);
            await taskfly.DeletePriority(newId);
        }

        private static async Task TaskTypeSample()
        {
            var type = await taskfly.GetTaskType();
            var newType = new TaskType()
            {
                Description = "Type Inserted"
            };
            int newId = await taskfly.AddTaskType(newType);
            newType.Id = newId;
            newType.Description = "Type Changed";
            await taskfly.ChangeTaskType(newType);
            await taskfly.DeleteTaskType(newId);
        }

        private static async Task TaskSample()
        {
            #region Get Task Custom Fields
            var customFields = await taskfly.GetTaskCustomFields();
            #endregion

            #region Create Task
            var board = (await taskfly.GetBoards()).First();
            var phase = (await taskfly.GetTaskPhases(board.Id)).First();
            var priority = (await taskfly.GetTaskPriority()).First();
            var taskType = (await taskfly.GetTaskType()).First();
            var customer = (await taskfly.GetCustomers()).First();
            var project = (await taskfly.GetProjects()).First();

            var newTask = new Tasks
            {
                Description = "Task Created",
                EstimatedDate = DateTime.Now.AddDays(10),
                PhaseId = phase.Id,
                PriorityId = priority.Id,
                TypeId = taskType.Id,
                CustomerId = customer.Id,
                ProjectId = project.Id,
                BoardId = board.Id
            };

            // You need to check Custom Fields - maybe you have required fields
            if (customFields.Count() > 0)
            {
                foreach (var f in customFields)
                {
                    var newField = new TaskAddCustomFields();
                    newField.CustomFieldId = f.Id;
                    switch (f.Type)
                    {
                        case "text":
                            {
                                newField.Value = "Text Sample Value";
                                break;
                            }
                        case "int":
                            {
                                newField.Value = "0";
                                break;
                            }
                        case "decimal":
                            {
                                newField.Value = "1.23";
                                break;
                            }
                        case "date":
                            {
                                newField.Value = DateTime.Today.ToShortDateString();
                                break;
                            }
                        case "bool":
                            {
                                newField.Value = "true";
                                break;
                            }
                        default:
                            break;
                    }
                    newTask.CustomFields.Add(newField);
                }
            }
            int newID = await taskfly.AddTask(newTask);
            #endregion

            #region Task Timer
            await taskfly.TaskStartTimer(newID);
            await Task.Delay(2000);
            await taskfly.TaskStopTimer(newID);
            #endregion

            #region Task Find
            var filter = new Dictionary<string, object>
            {
                {"Id", newID }
            };
            var task = taskfly.GetTasks(filter);
            #endregion

            #region Transfer Task to Another User
            var userTransfer = await taskfly.GetUsersToTransferTask();
            await taskfly.TransferTask(newID, userTransfer.First().UserId);
            #endregion

            #region Task Comments
            var taskComments = taskfly.GetTaskComments(newID);
            var comment = new TaskComment
            {
                PhaseId = null,
                SendToUserId = null,
                Description = "Commented using API"
            };
            await taskfly.SendTaskComments(newID, comment);
            #endregion

            #region Task Attachments
            var fileName = "TaskFlyImage.png";
            using (var fs = new FileStream(fileName, FileMode.Open))
            {

                byte[] bytearray = new byte[fs.Length];
                fs.Read(bytearray, 0, (int)fs.Length);

                var attachment = new TaskAttachment
                {
                    Name = fileName,
                    ByteArrayFile = bytearray
                };

                await taskfly.SendTaskAttachmment(newID, attachment);
            }
            #endregion

            #region Tasks by Tag
            var userTasks = taskfly.GetTasksByTag("<YOUR_TAG>");
            #endregion
        }
    }
}
