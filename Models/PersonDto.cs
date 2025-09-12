﻿namespace CarAPI.Models
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<CarDto> Cars { get; set; }
    }
}
