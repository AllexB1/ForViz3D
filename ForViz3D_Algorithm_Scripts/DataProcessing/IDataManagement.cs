using System.Collections.Generic;

namespace DataProcessing
{
    public interface IDataManagement
    {
        public List<string> GetNodes();
        public List<List<string>> GetEdges();
        public float FindEdgeForce(string sourceName, string targetName);
    }
}