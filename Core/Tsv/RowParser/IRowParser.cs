namespace Core.Tsv.RowParser;

public interface IRowParser
{
    void Parse(string row, RawRow raw);
}
