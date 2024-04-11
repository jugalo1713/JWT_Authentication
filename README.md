# JWT_Authentication

This project is a simple way to implement JWT

Note that this is mix based in this 2 tutorials

1- [Simple JWT Authentication using ASP.NET Core Web API](https://medium.com/@meghnav274/simple-jwt-authentication-using-asp-net-core-api-5d04b496d27b)  
2- [How to Implement JWT Authentication in Web API Using .Net 6.0, Asp.Net Core](https://www.c-sharpcorner.com/article/how-to-implement-jwt-authentication-in-web-api-using-net-6-0-asp-net-core/?__cf_chl_rt_tk=9zsdRyLJ6HY1QhhSsIBDgE5VuHxbM6QOhC4Tpw3G_Fk-1712809358-0.0.1.1-1514)

## Get token

Token is created in service `UserService` and is used in this endpoint: api/user/login

```CSharp
  public string Login(User user)
 {
     var loginUser = _users.SingleOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);

     if(loginUser == null)
     {
         return "";
     }

     var tokenHandler = new JwtSecurityTokenHandler();
     var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
     var tokenDescriptor = new SecurityTokenDescriptor
     {
         Subject = new ClaimsIdentity(new Claim[]
         {
             new Claim(ClaimTypes.Name, loginUser.UserName),
             new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
             new Claim(JwtRegisteredClaimNames.Aud,_configuration["Jwt:Audience"]),
             new Claim(JwtRegisteredClaimNames.Iss,_configuration["Jwt:Issuer"]),
             new Claim("UserId", user.UserId.ToString()),
             new Claim("DisplayName", user.DisplayName),
             new Claim("UserName", user.UserName),
             new Claim("Email", user.Email)

         }),
         Expires = DateTime.UtcNow.AddHours(1),
         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
     };

     var token = tokenHandler.CreateToken(tokenDescriptor);
     return tokenHandler.WriteToken(token);
 }
```

## Use Token

Once you get the token in the response can send it to this endpoint: api/user/getall

The code used to check on the JWT is

```CSharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
```
