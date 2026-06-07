using AutoMapper;
using SmartClinic.Application.DTOs;
using SmartClinic.Domain.Entities;

namespace SmartClinic.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientDto>().ReverseMap();
        CreateMap<Patient, CreatePatientDto>().ReverseMap();
        CreateMap<Patient, UpdatePatientDto>().ReverseMap();

        CreateMap<Clinic, ClinicDto>()
            .ForMember(dest => dest.DoctorCount, opt => opt.MapFrom(src => src.Doctors.Count))
            .ReverseMap();
        CreateMap<Clinic, CreateClinicDto>().ReverseMap();
        CreateMap<Clinic, UpdateClinicDto>().ReverseMap();

        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : string.Empty))
            .ReverseMap();
        CreateMap<Doctor, CreateDoctorDto>().ReverseMap();
        CreateMap<Doctor, UpdateDoctorDto>().ReverseMap();

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Phone : string.Empty))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
            .ForMember(dest => dest.DoctorSpecialty, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Specialty : string.Empty))
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : string.Empty))
            .ReverseMap();
        CreateMap<Appointment, CreateAppointmentDto>().ReverseMap();
        CreateMap<Appointment, UpdateAppointmentDto>().ReverseMap();

        CreateMap<QueueTicket, QueueTicketDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
            .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Phone : string.Empty))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
            .ForMember(dest => dest.DoctorSpecialty, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Specialty : string.Empty))
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : string.Empty))
            .ForMember(dest => dest.CalledByDoctorName, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<QueueTicket, CreateQueueTicketDto>().ReverseMap();

        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<Notification, SendNotificationDto>().ReverseMap();
    }
}