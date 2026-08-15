using JobScout.Domain.Models;

namespace JobScout.Application.Interfaces;

public interface ISkillExtractor
{
    CandidateProfile Extract(string cvText);
}
