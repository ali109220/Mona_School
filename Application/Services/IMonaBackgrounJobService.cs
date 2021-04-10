using System.Threading.Tasks;

namespace Application.Services
{
    public interface IMonaBackgrounJobService
    {
        Task<bool> UpdateObjStatus();
        Task<bool> SendNextLessonEmail();
    }
}