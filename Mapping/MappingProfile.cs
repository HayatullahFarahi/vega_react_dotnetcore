﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vega.Controllers.Resources;
using vega.Models;
using static vega.Controllers.Resources.VehicleResource;

namespace vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to API Resource
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Feature, FeatureResource>();
            CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource { Name = v.ContactName, Email = v.ContactEmail, Phone = v.ContactPhone }))
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));


            // API Resource to Domain
            CreateMap<VehicleResource, Vehicle>()
                .ForMember(vehicle => vehicle.Id, opt => opt.Ignore())
                .ForMember(vehicle => vehicle.ContactName, operation => operation.MapFrom(vehicleResource => vehicleResource.Contact.Name))
                .ForMember(vehicle => vehicle.ContactEmail, operation => operation.MapFrom(vehicleResource => vehicleResource.Contact.Email))
                .ForMember(vehicle => vehicle.ContactPhone, operation => operation.MapFrom(vehicleResource => vehicleResource.Contact.Phone))
                .ForMember(vehicle => vehicle.Features, operation => operation.Ignore())
                .AfterMap((vr, v) =>
                {
                    // Remove unselected features
                    var removedFeatures = v.Features.Where(f => !vr.Features.Contains(f.FeatureId));
                    foreach (var f in removedFeatures)
                        v.Features.Remove(f);
                    //Add new features
                    var addedFeatures = vr.Features.Where(id => !v.Features.Any(f => f.FeatureId == id)).Select(id => new VehicleFeature { FeatureId = id });
                    foreach (var f in addedFeatures)
                        v.Features.Add(f);
                });
        }
    }
}