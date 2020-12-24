﻿using DataAccessLibrary.DB;
using DataAccessLibrary.DB.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WEBApi.DTOs;

namespace WEBApi.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController:ControllerBase
    {
        private readonly IUserReadRepository _repo;
        private readonly IModelConverter _converter;

        public ProfileController(IUserReadRepository repo, IModelConverter converter)
        {
            _repo = repo;
            this._converter = converter;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("personal_data")]
        public async Task<ActionResult<UserInfoModel>> GetPersonalData()
        {
            string id = GetUserIdOfCurrentRequest();
            if (!String.IsNullOrEmpty(id))
            {
                User user = await _repo.FindUserByIdAsync(id);
                if (user is not null)
                {
                    UserInfoModel userInfo = _converter.ConvertUserToInfoModel(user);
                    return Ok(userInfo);
                }
                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }
        private string GetUserIdOfCurrentRequest()
        {
            return this.User.Claims.Where(
                x => x.Type.Equals(ClaimTypes.NameIdentifier)
                ).FirstOrDefault().Value;
        }
    }
}