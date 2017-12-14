using Jiguang.JPush.Model;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace Jiguang.JPush
{
    public class ScheduleClient
    {
        private const string BASE_URL = "https://api.jpush.cn";

        /// <summary>
        /// 创建定时任务。
        /// </summary>
        /// <param name="json">
        ///     自己构造的请求 json 字符串。
        ///     <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_push_schedule/#schedule"/>
        /// </param>
        public HttpResponse CreateScheduleTask(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            var url = BASE_URL + "/v3/schedules";
            HttpContent requestContent = new StringContent(json, Encoding.UTF8);
            var msgTask = JPushClient.HttpClient.PostAsync(url, requestContent);
            msgTask.Wait();
            var msg = msgTask.Result;
            var contentTask = msg.Content.ReadAsStringAsync();
            contentTask.Wait();
            string responseContent = contentTask.Result;
            return new HttpResponse(msg.StatusCode, msg.Headers, responseContent);
        }

        /// <summary>
        /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_push_schedule/#_4"/>
        /// 创建单次定时任务。
        /// </summary>
        /// <param name="name">表示 schedule 任务的名字，由 schedule-api 在用户成功创建 schedule 任务后返回，不得超过 255 字节，由汉字、字母、数字、下划线组成。</param>
        /// <param name="pushPayload">推送对象</param>
        /// <param name="trigger">触发器</param>
        public HttpResponse CreateSingleScheduleTask(string name, PushPayload pushPayload, string triggeringTime)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (pushPayload == null)
                throw new ArgumentNullException(nameof(pushPayload));

            if (string.IsNullOrEmpty(triggeringTime))
                throw new ArgumentNullException(nameof(triggeringTime));

            JObject requestJson = new JObject
            {
                ["name"] = name,
                ["enabled"] = true,
                ["push"] = JObject.FromObject(pushPayload),
                ["trigger"] = new JObject
                {
                    ["single"] = new JObject
                    {
                        ["time"] = triggeringTime
                    }
                }
            };
            return CreateScheduleTask(requestJson.ToString());
        }

        /// <summary>
        /// <see cref="CreatePeriodicalScheduleTask(string, PushPayload, Trigger)"/>
        /// 创建会在一段时间内重复执行的定期任务。
        /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_push_schedule/#_4"/>
        /// </summary>
        /// <param name="name">表示 schedule 任务的名字，由 schedule-api 在用户成功创建 schedule 任务后返回，不得超过 255 字节，由汉字、字母、数字、下划线组成。</param>
        /// <param name="pushPayload">推送对象</param>
        /// <param name="trigger">触发器</param>
        public HttpResponse CreatePeriodicalScheduleTask(string name, PushPayload pushPayload, Trigger trigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (pushPayload == null)
                throw new ArgumentNullException(nameof(pushPayload));

            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            JObject requestJson = new JObject
            {
                ["name"] = name,
                ["enabled"] = true,
                ["push"] = JObject.FromObject(pushPayload),
                ["trigger"] = new JObject()
                {
                    ["periodical"] = JObject.FromObject(trigger)
                }
            };
 
            return CreateScheduleTask(requestJson.ToString());
        }

        /// <summary>
        /// <see cref="GetValidScheduleTasks(int)"/>
        /// 获取有效的定时任务列表。
        /// </summary>
        /// <param name="page">
        ///     <para>返回当前请求页的详细的 schedule-task 列表，如未指定 page 则 page 为 1。</para>
        ///     <para>排序规则：创建时间，由 schedule-service 完成。</para>
        ///     <para>如果请求页数大于总页数，则 page 为请求值，schedules 为空。</para>
        ///     <para>每页最多返回 50 个 task，如请求页实际的 task 的个数小于 50，则返回实际数量的 task。</para>
        /// </param>
        public HttpResponse GetValidScheduleTasks(int page = 1)
        {
            if (page <= 0)
                throw new ArgumentNullException(nameof(page));

            var url = BASE_URL + "/v3/schedules?page=" + page;
            var msgTask = JPushClient.HttpClient.GetAsync(url);
            msgTask.Wait();
            var msg = msgTask.Result;
            var contentTask = msg.Content.ReadAsStringAsync();
            contentTask.Wait();
            string responseContent = contentTask.Result;
            return new HttpResponse(msg.StatusCode, msg.Headers, responseContent);
        }

        /// <summary>
        /// <see cref="GetScheduleTask(string)"/>
        /// 获取指定的定时任务。
        /// </summary>
        /// <param name="scheduleId">定时任务 ID。在创建定时任务时会返回。</param>
        public HttpResponse GetScheduleTask(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
                throw new ArgumentNullException(nameof(scheduleId));

            var url = BASE_URL + "/v3/schedules/" + scheduleId;
            var msgTask = JPushClient.HttpClient.GetAsync(url);
            msgTask.Wait();
            var msg = msgTask.Result;
            var contentTask = msg.Content.ReadAsStringAsync();
            contentTask.Wait();
            string responseContent = contentTask.Result;
            return new HttpResponse(msg.StatusCode, msg.Headers, responseContent);
        }

        public HttpResponse UpdateScheduleTask(string scheduleId, string json)
        {
            if (string.IsNullOrEmpty(scheduleId))
                throw new ArgumentNullException(nameof(scheduleId));

            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            var url = BASE_URL + "/v3/schedules/" + scheduleId;
            HttpContent requestContent = new StringContent(json, Encoding.UTF8);
            var msgTask = JPushClient.HttpClient.PutAsync(url, requestContent);
            msgTask.Wait();
            var msg = msgTask.Result;
            var contentTask = msg.Content.ReadAsStringAsync();
            contentTask.Wait();
            string responseContent = contentTask.Result;
            return new HttpResponse(msg.StatusCode, msg.Headers, responseContent);
        }

        /// <summary>
        /// <see cref="UpdateSingleScheduleTask(string, string, bool?, string, PushPayload)"/>
        /// 更新单次定时任务。
        /// </summary>
        /// <param name="scheduleId">任务 ID</param>
        /// <param name="name">任务名称，为 null 表示不更新。</param>
        /// <param name="enabled">是否可用，为 null 表示不更新。</param>
        /// <param name="triggeringTime">触发时间，类似 "2017-08-03 12:00:00"，为 null 表示不更新。</param>
        /// <param name="pushPayload">推送内容，为 null 表示不更新。</param>
        public HttpResponse UpdateSingleScheduleTask(string scheduleId, string name, bool? enabled,
            string triggeringTime, PushPayload pushPayload)
        {
            if (string.IsNullOrEmpty(scheduleId))
                throw new ArgumentNullException(scheduleId);

            JObject json = new JObject();

            if (!string.IsNullOrEmpty(name))
                json["name"] = name;

            if (enabled != null)
                json["enabled"] = enabled;

            if (triggeringTime != null)
            {
                json["trigger"] = new JObject
                {
                    ["single"] = new JObject
                    {
                        ["time"] = triggeringTime
                    }
                };
            }

            if (pushPayload != null)
            {
                json["push"] = JObject.FromObject(pushPayload);
            }

            return UpdateScheduleTask(scheduleId, json.ToString());
        }

        /// <summary>
        /// <see cref="UpdatePeriodicalScheduleTask(string, string, bool?, Trigger, PushPayload)"/>
        /// 更新会重复执行的定时任务。
        /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_push_schedule/#schedule_2"/>
        /// </summary>
        /// <param name="scheduleId">任务 ID</param>
        /// <param name="name">任务名称，为 null 表示不更新。</param>
        /// <param name="enabled">是否可用，为 null 表示不更新。</param>
        /// <param name="trigger">触发器对象，为 null 表示不更新。</param>
        /// <param name="pushPayload">推送内容，为 null 表示不更新。</param>
        public HttpResponse UpdatePeriodicalScheduleTask(string scheduleId, string name, bool? enabled,
            Trigger trigger, PushPayload pushPayload)
        {
            if (string.IsNullOrEmpty(scheduleId))
                throw new ArgumentNullException(scheduleId);

            JObject json = new JObject();

            if (!string.IsNullOrEmpty(name))
                json["name"] = name;

            if (enabled != null)
                json["enabled"] = enabled;

            if (trigger != null)
            {
                json["trigger"] = new JObject
                {
                    ["periodical"] = JObject.FromObject(trigger)
                };
            }

            if (pushPayload != null)
            {
                json["push"] = JObject.FromObject(pushPayload);
            }
            return UpdateScheduleTask(scheduleId, json.ToString());
        }

        /// <summary>
        /// <see cref="DeleteScheduleTask(string)"/>
        /// 删除指定的定时任务。
        /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_push_schedule/#schedule_3"/>
        /// </summary>
        /// <param name="scheduleId">已创建的 schedule 任务的 id。如果 scheduleId 不合法，即不是有效的 uuid，则返回 404。</param>
        public HttpResponse DeleteScheduleTask(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
                throw new ArgumentNullException(nameof(scheduleId));

            var url = BASE_URL + "/v3/schedules/" + scheduleId;
            var msgTask = JPushClient.HttpClient.DeleteAsync(url);
            msgTask.Wait();
            var msg = msgTask.Result;
            var contentTask = msg.Content.ReadAsStringAsync();
            contentTask.Wait();
            string responseContent = contentTask.Result;
            return new HttpResponse(msg.StatusCode, msg.Headers, responseContent);
        }

    }
}
