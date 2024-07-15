using Newtonsoft.Json;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Models;
using SimpleChat.Library.Requests.Users;
using SimpleChat.Library.Responses;

namespace SimpleChat.Library.Clients
{
    public class UsersClinet
    {
        public HttpClient HttpClient { get; set; }
        public string ControllerBase => ControllerNames.Users;
        public UsersClinet(HttpClient client, string baseUrl)
        {
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<ServiceResponse<Guid>> Add(User user)
        {
            var jsonContent = JsonConvert.SerializeObject(user);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var requestResult = await HttpClient.PostAsync($"{ControllerBase}/{RepoActions.Add}", httpContent);
            var response = await HandleResponse<Guid>(requestResult);
            return response;
        }

        public async Task<ServiceResponse<object>> Remove(Guid id)
        {
            var requestResult = await HttpClient.DeleteAsync($"{ControllerBase}/{RepoActions.Remove}?id={id}");
            var response = await HandleResponse<object>(requestResult);
            return response;
        }

        public async Task<ServiceResponse<User>> Update(UpdateUserRequest request)
        {
            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            var requestResult = await HttpClient.PutAsync($"{ControllerBase}/{RepoActions.Update}", httpContent);
            var response = await HandleResponse<User>(requestResult);
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<User>>> GetAll()
        {
            var requestResult = await HttpClient.GetAsync($"{ControllerBase}/{RepoActions.GetAll}");
            var response = await HandleResponse<IEnumerable<User>>(requestResult);
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<User>>> GetAll(string name)
        {
            var requestResult = await HttpClient.GetAsync($"{ControllerBase}/{RepoActions.GetAll}/{name}");
            var response = await HandleResponse<IEnumerable<User>>(requestResult);
            return response;
        }

        public async Task<ServiceResponse<User>> GetOne(Guid id)
        {
            var requestResult = await HttpClient.GetAsync($"{ControllerBase}?id={id}");
            var response = await HandleResponse<User>(requestResult);
            return response;
        }

        protected virtual async Task<ServiceResponse<TResponse>> HandleResponse<TResponse>(HttpResponseMessage requestResult)
        {
            ServiceResponse<TResponse> result;

            if (requestResult.IsSuccessStatusCode)
            {
                var responseJson = await requestResult.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<TResponse>(responseJson);
                result = new ServiceResponse<TResponse>(responseObject);
            }
            else
            {
                result = new ServiceResponse<TResponse>(new[] { $"{nameof(requestResult.StatusCode)}:{requestResult.StatusCode}; " +
                    $"{nameof(requestResult.ReasonPhrase)}:{requestResult.ReasonPhrase}." });
            }

            return result;
        } 
    }
}
