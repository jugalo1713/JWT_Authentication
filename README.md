# JWT_Authentication

This project is a simple project that uses JWT

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