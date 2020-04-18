namespace SettlersForVillages.Models
{
    public class MilitiaMoveTierModel
    {
        public float MilitiaToAdd { get; }

        public MilitiaMoveTierModel(float militiaToAdd)
        {
            MilitiaToAdd = militiaToAdd;
        }

        public string GetOptionText()
        {
            return "Move " + MilitiaToAdd + " militia from villages";
        }

        public string GetOptionId()
        {
            return "castle_villages_militia_move_" + MilitiaToAdd;
        }
    }
}