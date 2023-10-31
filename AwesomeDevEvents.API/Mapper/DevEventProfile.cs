﻿using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;

namespace AwesomeDevEvents.API.Mapper
{
    public class DevEventProfile : Profile
    {
        public DevEventProfile()
        {
            CreateMap<DevEvent, DevEventViewModel>(); 
            CreateMap<DevEventSpeaker, DevEventSpeakerViewModel>();

            CreateMap<DevEventInputModel, DevEvent>();
            CreateMap<DevEventSpeakerInputModel, DevEventSpeaker>();
        }
    }
}
