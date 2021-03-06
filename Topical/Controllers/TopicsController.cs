﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using Topical.Models;
using Topical.Services;

namespace Topical.Controllers
{
    public class TopicsController : ApiController
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        //// GET api/Topical/5
        public IHttpActionResult GetTopic(string id)
        {
            var topic = _topicService.GetTopic(id);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        public IHttpActionResult CreateTopic(Topic topic)
        {
            if (String.IsNullOrEmpty(topic.Title))
            {
                return BadRequest();
            }

            _topicService.Create(topic);
            return Ok(topic);
        }

        public IEnumerable<Topic> GetTopic([FromUri]TopicFilter filter)
        {
            filter = filter ?? new TopicFilter();
            return _topicService.GetTopics(filter);
        }
    }
}