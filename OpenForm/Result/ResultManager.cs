using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Result
{
    class ResultManager
    {
        public List<Response> Result { get; set; }
        public int threshold { get; set; }
        public ResultManager(int threshold)
        {
            Result = new List<Response>();
            this.threshold = threshold;
        }
        public int isFilled(string ident, int row, int col)
        {
            Response r = Result.Find(x => x.Ident == ident && x.Row == row && x.Col == col);
            return r.Score > threshold?1:-1;
        }
        public int getRowAnswer(string ident, int row)
        {
            Response r = Result.FindAll(x => x.Ident == ident && x.Row==row).OrderByDescending(x => x.Score).First();
            if (r.Score > threshold) return r.Col; else return -1;
        }
        public int getColAnswer(string ident, int col)
        {
            Response r = Result.FindAll(x => x.Ident == ident && x.Col == col).OrderByDescending(x => x.Score).First();
            if (r.Score > threshold) return r.Row; else return -1;
        }
    }
}
