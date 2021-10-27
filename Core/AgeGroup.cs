using System.Collections.Generic;

#nullable disable
namespace Core
{
    public class AgeGroup
    {
        public int AgeGroupId { get; set; }
        public string Label { get; set; }
        public int? BoundUpper { get; set; }
        public int? BoundLower { get; set; }
    }
}
