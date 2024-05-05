using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly List<string[]> _users;
        private static List<User> localDbUser = new List<User>();
        public UserController()
        {
            _users = new List<string[]>();
            _users.Add(new[] { "Ibrahim Halil Ates", "ibrahimhates.png" });
            _users.Add(new[] { "Muhammed Furkan Conoglu", "furkanconoglu.jpeg" });
            _users.Add(new[] { "Burakhan Kurt", "burakhankurt.jpeg" });
            _users.Add(new[] { "Omer Faruk Soydemir", "omerfaruksoydemir.jpeg" });
        }

        [HttpGet]
        public IActionResult AllUser()
        {
            localDbUser.Clear();

            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                var randomUserIndex = random.Next(0, _users.Count);
                localDbUser.Add(new()
                {
                    Id = i + 1,
                    Name = _users[randomUserIndex][0],
                    UserName = _users[randomUserIndex][1].Split(".")[0],
                    ProfilePicture = _users[randomUserIndex][1],
                });
            }

            return Ok(localDbUser);// 200 basarili // 404 notfound // 400 badrequest // 500 internel server
        }

        [HttpGet("filter")]
        public IActionResult FilterAllUser([FromQuery] string searchTerm)
        {
            var searchTermList = localDbUser
                .Where(x => x.UserName.Contains(searchTerm)
                    || x.Name.Contains(searchTerm))
                .ToList();

            return Ok(searchTermList);
        }

    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
    }
}
