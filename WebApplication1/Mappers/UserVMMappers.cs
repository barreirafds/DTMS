using BusinessLogicLayer.DTOs;

namespace DTMS.Mappers
{
    public class UserVMMappers
    {
        public static ViewModels.UserVM ToViewModel(UserDTO userDTO)
        {
            return new ViewModels.UserVM
            {
                Id = userDTO.Id,
                Username = userDTO.Username,
                Password = userDTO.Password,
                Role = userDTO.Role
            };
        }

        public static List<ViewModels.UserVM> ToViewModelList(List<UserDTO> userDTOs)
        {
            return userDTOs.Select(ToViewModel).ToList();
        }
    }
}

