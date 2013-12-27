using System;
using System.Collections.Generic;
using System.Web.Http;
using Topical.Models;
using Topical.Services;

namespace Topical.Controllers
{
    [RoutePrefix("api/topics/{topicId}")]
    public class TagsController : ApiController
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [Route("tags")]
        public IHttpActionResult Get(string topicId)
        {
            if (String.IsNullOrEmpty(topicId))
            {
                return BadRequest();
            }
            return Ok(_tagService.GetTags(topicId));
        }

        [Route("tag/{tagId}")]
        public IHttpActionResult Post(string topicId, string tagId, int vote)
        {
            if (String.IsNullOrEmpty(topicId))
            {
                return BadRequest();
            }
            else if (String.IsNullOrEmpty(tagId))
            {
                return BadRequest();
            }

            return Ok((object)null);
        }
    }
}