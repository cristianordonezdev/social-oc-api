using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.Chats
{
    public class SendMessageDto
    {
        [Required]
        public string TextMessage { get; set; }
    }
}
