using BusinessLogicLayer.DTOs;

namespace DTMS.Mappers
{
    public class TableVMMappers
    {
       public static ViewModels.TableVM ToViewModel(TableDTO tableDTO)
        {
            return new ViewModels.TableVM
            {
                Id = tableDTO.Id,
                Number = tableDTO.Number,
                Seats = tableDTO.Seats,
                Status = tableDTO.Status
            };
        }
    }
}
