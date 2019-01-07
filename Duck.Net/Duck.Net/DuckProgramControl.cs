using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duck.Net
{
    class DuckProgramControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<String> LineList { get; set; }
        public Dictionary<String, int> LabelList { get; set; }
        public int EIP { get; set; }
        public Dictionary<String, String> GlobalVars { get; set; }
        Dictionary<String, List<String>> GlobalList { get; set; }

        public DuckProgramControl()
        {
            this.LineList = new List<String>();
            this.LabelList = new Dictionary<String, int>();
            this.EIP = 0;
            this.GlobalVars = new Dictionary<String, String>();
            this.GlobalList = new Dictionary<string, List<string>>();
        }

        public void loadProgram(String fileName)
        {
            String methodName = "loadProgram";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                String line = "";
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                int counter = 0;
                while ((line = file.ReadLine()) != null)
                {

                    String labelName = string.Empty;
                    labelName = catchLabel(line);
                    if (!string.IsNullOrEmpty(labelName))
                        this.LabelList[line] = counter;
                    this.LineList.Add(line);
                }

                file.Close();

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }



        }
        public int compareFunction(List<String> lineIstruction,
                          
                           ref bool IPIsChanged,
                            String compareType)
        {

            String methodName = "compareFunction";

            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                string term1 = lineIstruction[1];
                string term2 = lineIstruction[2];
                string jumpLabel = lineIstruction[3];




                if (Helper.isString(term1))
                    term1 = Helper.extractStringContent(term1);
                else
                    if (!Helper.isNumber(term1))
                    term1 = this.GlobalVars[term1];

                if (Helper.isString(term2))
                    term2 = Helper.extractStringContent(term2);
                else
                    if (!Helper.isNumber(term2))
                    term2 = this.GlobalVars[term2];

                int jumpTo = 0;

                if (!Helper.isNumber(jumpLabel))
                    jumpTo = this.LabelList[jumpLabel];
                else
                    jumpTo = Int32.Parse(jumpLabel);



                if (
                   (compareType.Equals("==") && (term1.Equals(term2))) ||
                   (compareType.Equals("!=") && (!term1.Equals(term2))) ||
                    (compareType.Equals(">") && (Int32.Parse(term1) > Int32.Parse(term2))) ||
                    (compareType.Equals(">=") && (Int32.Parse(term1) >= Int32.Parse(term2))) ||
                   (compareType.Equals("<") && (Int32.Parse(term1) < Int32.Parse(term2))) ||
                   (compareType.Equals("<=") && (Int32.Parse(term1) < Int32.Parse(term2)))
                   )
                {
                    EIP = jumpTo;
                    IPIsChanged = true;
                }




                IPIsChanged = false;

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));


            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
            return EIP;
        }

        public void subAssegnamento(String destinazione, String sorgente)
        {
            String methodName = "subAssegnamento";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                if (Helper.isString(sorgente))
                {
                    this.GlobalVars[destinazione] = Helper.extractStringContent(sorgente);
                }
                else if (Helper.isNumber(sorgente))
                {
                    this.GlobalVars[destinazione] = sorgente;
                }
                else
                    this.GlobalVars[destinazione] = this.GlobalVars[sorgente];

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void subAssegnamentoList(String destinazione, String sorgente, int index)
        {
            String methodName = "subAssegnamentoList";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                if (Helper.isString(sorgente))
                {
                    if (index == -1)
                        this.GlobalList[destinazione].Add(Helper.extractStringContent(sorgente));
                    else
                        this.GlobalList[destinazione][index] = Helper.extractStringContent(sorgente);
                }
                else if (Helper.isNumber(sorgente))
                {
                    if (index == -1)
                        this.GlobalList[destinazione].Add(sorgente);
                    else
                        this.GlobalList[destinazione][index] = sorgente;
                }
                else
                   if (index == -1)
                    this.GlobalList[destinazione].Add(this.GlobalVars[sorgente]);
                else
                    this.GlobalList[destinazione][index] = this.GlobalVars[sorgente];


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }

        }
        public void subPrint(List<String> lineIstruction, bool endLine)
        {

            String methodName = "subPrint";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                if (lineIstruction.Count > 1)
                {
                    string input = lineIstruction[1];
                    if (Helper.isString(input))
                    { //start of string match

                        if (!endLine)
                            Console.Write(input);
                        else
                            Console.WriteLine(input);

                    }
                    else
                    {
                        input = this.GlobalVars[input];
                        string outPut;
                        if (Helper.isString(input))
                            outPut = Helper.extractStringContent(input);
                        else
                            outPut = input;
                        if (!endLine)
                            Console.Write(outPut);
                        else
                            Console.WriteLine(outPut);
                    }

                }
                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }





        }
        public String catchLabel(String istruction)
        {

            String methodName = "catchLabel";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                if (istruction.Count() > 1 && istruction[0] == ':')
                    return istruction.Substring(1);

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
                return string.Empty;


            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }



        }
        public int subGoto(List<String> istruction)
        {
            String methodName = "subGoto";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                int line = Int32.Parse(istruction[1]);
                if (line > 0)
                    return line;

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
                return this.LabelList[istruction[1]];


            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void subOper(List<String> lineIstruction, char oper)
        {
            String methodName = "subOper";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                string varName = lineIstruction[1];
                string term1 = lineIstruction[2];
                string term2 = lineIstruction[3];

                long mTerm1 = 0;
                long mTerm2 = 0;
                long result = 0;
                if (Helper.isNumber(term1))
                    mTerm1 = Int64.Parse(term1);
                else
                    mTerm1 = Int64.Parse(this.GlobalVars[term1]);

                if (Helper.isNumber(term2))
                    mTerm2 = Int64.Parse(term2);
                else
                    mTerm2 = Int64.Parse(this.GlobalVars[term2]);

                if (oper == '+')
                    result = mTerm1 + mTerm2;
                else if (oper == '-')
                    result = mTerm1 - mTerm2;
                else if (oper == '*')
                    result = mTerm1 * mTerm2;
                else if (oper == '/')
                    result = mTerm1 / mTerm2;
                else if (oper == '%')
                    result = mTerm1 % mTerm2;



                subAssegnamento(varName, result.ToString());

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }

        }
        public void createList(List<String> lineIstruction)
        {
            String methodName = "createList";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                List<String> spltTerm = new List<String>();
                if (lineIstruction.Count() > 3)
                    spltTerm = new List<String>(lineIstruction[3].Split(','));
                string listName = lineIstruction[2];

                List<String> newList = new List<String>();

                for (int i = 0; i < spltTerm.Count(); i++)
                {
                    if (Helper.isNumber(spltTerm[i]))
                        newList.Add(spltTerm[i]);
                    else if (Helper.isString(spltTerm[i]))
                        newList.Add(Helper.extractStringContent(spltTerm[i]));
                    else
                        newList.Add(this.GlobalVars[spltTerm[i]]);


                }
                this.GlobalList.Add(listName, newList);

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }

        }
        public void listSize(List<String> lineIstruction)
        {
            String methodName = "listSize";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                subAssegnamento(lineIstruction[2], this.GlobalList[lineIstruction[3]].Count.ToString());

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void listRead(List<String> lineIstruction)
        {


            String methodName = "listRead";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                string varAssegnamento = lineIstruction[2];
                string listName = lineIstruction[3];
                string listIndex = lineIstruction[4];


                int inx = 0;
                if (!Helper.isNumber(listIndex))
                    inx = Int32.Parse(this.GlobalVars[listIndex]);
                else
                    inx = Int32.Parse(listIndex);


                subAssegnamento(varAssegnamento, this.GlobalList[listName][inx]);


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }



        }
        public void listWrite(List<String> lineIstruction)
        {
            String methodName = "listWrite";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                string listName = lineIstruction[2];
                string listIndex = lineIstruction[3];
                string value = lineIstruction[4];


                subAssegnamentoList(listName, value, Int32.Parse(listIndex));


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void listAdd(List<String> lineIstruction)
        {
            String methodName = "listAdd";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));


                string listName = lineIstruction[2];
                string value = lineIstruction[3];

                subAssegnamentoList(listName, value, -1);
                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }

        }
        public void readFromKeyboad(List<String> lineIstruction)
        {
            String methodName = "readFromKeyboad";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                string varName = lineIstruction[2];
                string tmpInput = Console.ReadLine();

                subAssegnamento(varName, tmpInput);

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void openFileText(List<String> lineIstruction, Dictionary<String, String> globalVars)
        {
            String methodName = "openFileText";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void closeFileText(List<String> lineIstruction, Dictionary<String, String> globalVars)
        {
            String methodName = "closeFileText";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void subExit(List<String> lineIstruction)
        {
            String methodName = "subExit";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                if (lineIstruction.Count() > 1)
                {
                    Environment.Exit((Int32.Parse(lineIstruction[1])));
                }
                else
                {
                    Environment.Exit(0);
                }


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void loadTextFile(List<String> lineIstruction)
        {
            String methodName = "loadTextFile";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                string listName = lineIstruction[2];
                string path = lineIstruction[3];
                List<String> lineList = new List<String>();


                String line = "";
                System.IO.StreamReader file = new System.IO.StreamReader(path);

                while ((line = file.ReadLine()) != null)
                {

                    lineList.Add("\"" + line + "\"");
                }

                file.Close();

                this.GlobalList[listName] = lineList;


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void iteratorFunction(List<String> lineIstruction)
        {
            String methodName = "iteratorFunction";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                this.GlobalVars["[ITERLIST]"] = lineIstruction[1];
                this.GlobalVars["[ITERSTART]"] = this.LabelList[lineIstruction[2]].ToString();
                this.GlobalVars["[INLOOPING]"] = "1";
                this.GlobalVars["[ITERCOUNTER]"] = "0";
                this.EIP = this.LabelList[lineIstruction[2]];

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public String catchIterator(String istruction)
        {
            String methodName = "catchIterator";
            string listName = string.Empty;
            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                int pos = 0;
                String iteratorName = String.Empty;
                pos = istruction.IndexOf("[ITERATOR ");
                if (pos >= 0)
                {

                    iteratorName = istruction.Split(' ')[1].Replace("]", "").Trim(); ;
                }

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
                return iteratorName;

            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void enableIteration(String iteratorListName)
        {
            String methodName = "enableIteration";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                int listSize = this.GlobalList[iteratorListName].Count;



                this.GlobalVars["[ITERLIST]"] = iteratorListName;
                this.GlobalVars["[ITERSTART]"] = EIP.ToString();

                this.GlobalVars["[ITERCOUNTER]"] = "0";
                this.GlobalVars["[ITERLISTSIZE]"] = listSize.ToString();

                if (listSize <= 0)
                {
                    this.GlobalVars["[CURRENT]"] = "";
                }
                else
                {
                    this.GlobalVars["[CURRENT]"] = this.GlobalList[iteratorListName][0];
                }


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void checkIterationEnd()
        {
            String methodName = "checkIterationEnd";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                string iteratorListName = this.GlobalVars["[ITERLIST]"];
                int listSize = this.GlobalList[iteratorListName].Count;
                int actualIterCounter = Int32.Parse(this.GlobalVars["[ITERCOUNTER]"]);

                actualIterCounter++;
                if (actualIterCounter < listSize)
                {

                    this.GlobalVars["[ITERCOUNTER]"] = actualIterCounter.ToString();
                    this.GlobalVars["[CURRENT]"] = this.GlobalList[iteratorListName][actualIterCounter];

                    EIP = Int32.Parse(this.GlobalVars["[ITERSTART]"]);

                }
                else
                {
                    EIP++;
                }


                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public bool catchEndIterator(String istruction)
        {
            String methodName = "catchEndIterator";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                if (istruction.Equals("[END ITERATOR]"))
                {
                    return true;
                }

                else
                {
                    log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
                    return false;
                }



            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }

    }
}
