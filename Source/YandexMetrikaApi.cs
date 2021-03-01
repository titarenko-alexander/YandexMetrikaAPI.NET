using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace YandexMetrikaApi
{
    static class HTTP
    {
        public static ByteArrayContent CreateBody(object body)
        {
            return (new ByteArrayContent((System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body)))));
        }
    }

    namespace Counters
    {
        public class Counter
        {
            public JObject GetCounters(string token)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var result = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counters");
                var content = result.Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetInfoAboutCounter(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var result = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}");
                var content = result.Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateCounter(string token, string counterName, string siteDomain)
            {
                var buffer = HTTP.CreateBody(new {counter = new {name = counterName, site2 = siteDomain}});

                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var result = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counters?{counterName}", buffer);
                var content = result.Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
            
            public JObject EditCounter(string token, string counterId, string newCounterName, string newSiteDomain = "")
            {
                var buffer = newSiteDomain == "" ? HTTP.CreateBody(new {counter = new {name = newCounterName}}) : HTTP.CreateBody(new {counter = new {name = newCounterName, site2 = newSiteDomain}});

                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var result = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}", buffer);
                var content = result.Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteCounter(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject RecreateCounter(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/undelete", null).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }


        }
    }

    namespace Goals
    {
        public class Goal
        {
            public JObject GetGoals(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditGoals(string token, string counterId, string goalId, string newGoalName)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {id = goalId, name = "goal"});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goal/{goalId}", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                try
                {
                    return JObject.Parse(content);
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Service unavailable or wrong JSON");
                    return null;
                }
                
            }
            
            public JObject CreateJSGoal(string token, string counterId, string goalName, string conditionType, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "action", is_retargeting = 0, conditions = new List<Object>() {new {type = conditionType, url = url}}}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateNumberGoal(string token, string counterId, string goalName, string depth)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "number", is_retargeting = 0, depth = depth}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            /// <summary> 
            /// For more information: https://yandex.ru/dev/metrika/doc/api2/management/goals/class_goale.htm
            /// </summary>
            public JObject CreateURLGoal(string token, string counterId, string goalName, string conditionType, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "url", is_retargeting = 0, conditions = new List<Object>() {new {type = conditionType, url = url}}}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreatePhoneGoal(string token, string counterId, string goalName, string conditionType, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "phone", is_retargeting = 0, conditions = new List<Object> () {new {type = conditionType, url = url}}}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateEmailGoal(string token, string counterId, string goalName, string conditionType, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "email", is_retargeting = 0, conditions = new List<Object>() {new {type = conditionType, url = url}}}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            ///<summary>ConditionType = form_name or form_id</summary>
            public JObject CreateFormGoal(string token, string counterId, string goalName, string conditionType, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "form", is_retargeting = 0, conditions = new List<Object>() {new {type = conditionType, url = url}}}});
                System.Console.WriteLine(body.ReadAsStringAsync().Result);
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateMessengerGoal(string token, string counterId, string goalName, string url)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {goal = new {name = goalName, type = "messenger", is_retargeting = 0, conditions = new List<Object>() {new {type = "messenger", url = url}}}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/goals", body).Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetMessengers(string token)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/messengers").Result.Content.
                ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Filters
    {
        public class Filter
        {
            public JObject GetFilters(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/filters").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetFilterInformation(string token, string counterId, string filterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/filter/{filterId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateFilter(string token, string counterId, string attr, string type, string value, string action, string status)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {filter = new {attr = attr, type = type, value = value, action = action, status = status}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/filters", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditFilter(string token, string counterId, string filterId, string attr, string type, string value, string action, string status)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {filter = new {attr = attr, type = type, value = value, action = action, status = status}});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/filter/{filterId}", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteFilter(string token, string counterId, string filterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/filter/{filterId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Operations
    {
        public class Operation
        {
            public JObject GetOperations(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/operations").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetOperationInformation(string token, string counterId, string operationId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/operations").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateOperation(string token, string counterId, string action, string attr, string value, string status)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {operation = new {action = action, attr = attr, value = value, status = status}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/operations", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditOperation(string token, string counterId, string operationId,string action, string attr, string value, string status)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {operation = new {action = action, attr = attr, value = value, status = status}});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/operation/{operationId}", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteOperation(string token, string counterId, string operationId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/operation/{operationId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Permissions
    {
        public class Permission
        {
            public JObject GetPermissions(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/grants").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetPermissionInformation(string token, string counterId, string userLogin)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/grant?user_login={userLogin}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreatePublicPermission(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {grant = new {perm = "public_stat"}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/public_grant", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeletePublicPermission(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/public_grant").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateIndividualPermission(string token, string counterId, string user_login, string perm, string comment, Boolean partnet_data_access)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {grant = new {user_login = user_login, perm = perm, comment = comment, partnet_data_access = partnet_data_access}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/grants", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditIndividulPermission(string token, string counterId, string user_uid, string user_login, string perm, string comment, Boolean partnet_data_access)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {grant = new {user_login = user_login, perm = perm, comment = comment, partnet_data_access = partnet_data_access}});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/grant", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteIndividualPermission(string token, string counterId, string user_login)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/grant?user_login={user_login}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Accounts
    {
        public class Account
        {
            public JObject GetAccounts(string token)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/accounts").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteAccount(string token, string user_login)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/account?user_login={user_login}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public void EditAccounts(string token)
            {
                System.Console.WriteLine("This method doesn't work right now");
            }
        }
    }

    namespace Representative
    {
        public class Representative
        {
            public JObject GetRepresentative(string token)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/delegates").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteRepresentative(string token, string user_login)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/delegate?user_login={user_login}").Result.Content.ReadAsStringAsync().Result;
                return JObject.Parse(content);
            }

            public JObject AddRepresentative(string token, string user_login, string comment)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {@delegate = new {user_login = user_login, comment = comment}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/delegates", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Clients
    {
        public class Client
        {
            public JObject GetClients(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/clients?counters={counterId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Labels
    {
        public class Label
        {
            public JObject GetLabels(string token)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/labels").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject GetLabelInformation(string token, string labelId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/label/{labelId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateLabel(string token, string labelName)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {label = new {name = labelName}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/labels", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditLabel(string token, string labelId, string newLabelName)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {label = new {name = newLabelName}});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/label/{labelId}", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteLabel(string token, string labelId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/label/{labelId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject BindLabelToCounter(string token, string counterId,string labelId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/label/{labelId}", null).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject UnbindLabelToCounter(string token, string counterId, string labelId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/label/{labelId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace Segments
    {
        public class Segment
        {
            public JObject GetSegments(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/apisegment/segments").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
            
            public JObject GetSegmentInformation(string token, string counterId, string segmentId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/apisegment/segment/{segmentId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject CreateSegment(string token, string counterId, string segmentName, string expression, string segment_source)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {segment = new {name = segmentName, expression = expression, segment_source = segment_source}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/apisegment/segments", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject EditSegment(string token, string counterId, string segmentId, string segmentName, string expression, string segment_source)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {segment = new {name = segmentName, expression = expression, segment_source = segment_source}});
                var content = client.PutAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/apisegment/segment/{segmentId}", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject DeleteSegment(string token, string counterId, string segmentId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.DeleteAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/apisegment/segment/{segmentId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }

    namespace VisitorParameters
    {
        public class VisitorParameters
        {
            public JObject GetParameters(string token, string counterId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/user_params/uploadings").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
            
            public JObject GetParameterById(string token, string counterId, string uploadingId)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/user_params/uploading/{uploadingId}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }

            public JObject VerifyParametersUpload(string token, string counterId, string uploadingId, string content_id_type, string action, string status)
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var body = HTTP.CreateBody(new {uploading = new {content_id_type = content_id_type, action = action, status = status}});
                var content = client.PostAsync($"https://api-metrika.yandex.net/management/v1/counter/{counterId}/user_params/uploading/{uploadingId}/confirm", body).Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
            //Upload parameters, editUploadParameters missing
        }
    }

    namespace Reports
    {
        public class Reports
        {
            public JObject GetTable(string token, string counterId, string metrics, string date1 = "", string date2 = "")
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                var content = client.GetAsync($"https://api-metrika.yandex.net/stat/v1/data?ids={counterId}&metrics={metrics}&date1={date1}&date2={date2}").Result.Content.ReadAsStringAsync().Result;
                client.Dispose();
                return JObject.Parse(content);
            }
        }
    }
    
    namespace OfflineConversion
    {
        //Missing all methods
    }

    namespace CallControl
    {
        //Missing all methods
    }

    namespace AdvertisingCostUpload
    {
        //Missing all methods
    }

    namespace Graph
    {
        //Missing all methods
    }
}
