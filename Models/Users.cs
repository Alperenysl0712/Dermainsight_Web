﻿using Newtonsoft.Json;

namespace Dermainsight.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class CurrentUser
    {
        public static Users? activeUser {  get; set; }
    }
}
