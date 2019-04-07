using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public Dictionary<String, List<String>> GlobalList { get; set; }
        public Token LastToken { get; set; }
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
                    labelName = catchLabel(line.Trim());
                    if (!string.IsNullOrEmpty(labelName))
                        this.LabelList[labelName] = counter;
                    this.LineList.Add(line.Trim());
                    counter++;
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

        public void catchAssignment(Tokenizer scanner, Token setToken)
        {
            String methodName = "catchAssignment";
            Stack<Token> stack = new Stack<Token>();
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                Token actualToken = scanner.getNextToken(LexerAction.CONSUME_TOKEN);
                LastToken = setToken;
                while (true)
                {
                    if (actualToken.Name == TokenName.SPACE)
                    {
                        
                        actualToken = scanner.getNextToken(LexerAction.CONSUME_TOKEN);
                        continue;
                    }
                    if (actualToken.Name == TokenName.VAR && LastToken.Name == TokenName.SET)
                    {
                        LastToken = actualToken;
                        stack.Push(actualToken);

                        actualToken = scanner.getNextToken(LexerAction.CONSUME_TOKEN);
                        continue;
                    }
                    if((actualToken.Name == TokenName.VAR || actualToken.Name == TokenName.STRING || actualToken.Name == TokenName.NUMBER) && LastToken.Name == TokenName.ASSIGN )
                    {
                        String sourceVar = actualToken.Value;
                        Token assignToken = stack.Pop();
                        Token destinationTokenVar = stack.Pop();
                        String destinationVar = destinationTokenVar.Value;

                        subAssegnamento(destinationVar, sourceVar);
                        return;

                    }
                    if (actualToken.Name == TokenName.ASSIGN && LastToken.Name == TokenName.VAR)
                    {
                        LastToken = actualToken;
                        stack.Push(actualToken);

                        actualToken = scanner.getNextToken(LexerAction.CONSUME_TOKEN);
                        continue;
                    }
                    throw new Exception("Invalid assignment command");
                }                
                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        //public string catchProcedure(string istruction)
        //{
        //    String methodName = "catchProcedure";
        //    try
        //    {
        //        log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
        //        int pos = 0;
        //        String iteratorName = String.Empty;
        //        pos = istruction.IndexOf("[FUNCTION ");
        //        if (pos >= 0)
        //        {

        //            iteratorName = istruction.Split(' ')[1].Replace("]", "").Trim(); ;
        //        }

        //        log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
        //        return iteratorName;

        //    }
        //    catch(Exception ex)
        //    {
        //        log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
        //        throw ex;
        //    }
        //}

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
                string elseJump = lineIstruction.Count > 4 ? lineIstruction[4] : string.Empty;



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
                int jumpToElse = 0;

                if (!Helper.isNumber(jumpLabel))
                    jumpTo = this.LabelList[jumpLabel];
                else
                    jumpTo = Int32.Parse(jumpLabel);

                if (!string.IsNullOrEmpty(elseJump))
                {
                    if (!Helper.isNumber(elseJump))
                        jumpToElse = this.LabelList[elseJump];
                    else
                        jumpToElse = Int32.Parse(elseJump);
                }


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
                else {

                    if (jumpLabel != string.Empty)
                    {
                        EIP = jumpToElse;
                        IPIsChanged = true;
                    }
                    else
                    {
                        IPIsChanged = false;
                    }
                }


               

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));


            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
            return EIP;
        }

        public void printObject(string splt)
        {
            try
            {
                string var = splt.Substring(splt.IndexOf("@")+1);
                Console.WriteLine(var);
                if (!string.IsNullOrEmpty(var))
                {
                    if (GlobalVars.ContainsKey(var))
                        Console.WriteLine(GlobalVars[var]);
                    else if (GlobalList.ContainsKey(var))
                    {
                        foreach (string cc in GlobalList[var]) {
                            Console.WriteLine(cc);
                        }
                    }
                }
                return;
            }
            catch(Exception ex)
            {
                return;
            }
            
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

        public void stringToList(List<string> splt)
        {
            String methodName = "stringToList";
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                String destinationList = splt[2];
                String sourceStr = splt[3];

                if (Helper.isString(sourceStr))
                    sourceStr = Helper.extractStringContent(sourceStr);
                else if (!Helper.isNumber(sourceStr))
                    sourceStr = GlobalVars[sourceStr];

                List<String> nwList = new List<String>();
                for(int i = 0; i<sourceStr.Length; i++)
                {
                    nwList.Add(sourceStr[i].ToString());
                }

                GlobalList[destinationList] = nwList;
                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch (Exception ex)
            {

                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void stringAppend(List<string> splt)
        {
            String methodName = "stringAppend";
            try {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                String destinationVar = splt[2];
                String destinationStr = splt[3];
                String sourceStr = splt[4];

                if (Helper.isString(destinationStr))
                    destinationStr = Helper.extractStringContent(destinationStr);
                else if (!Helper.isNumber(destinationStr))
                    destinationStr = GlobalVars[destinationStr];

                if (Helper.isString(sourceStr))
                    sourceStr = Helper.extractStringContent(sourceStr);
                else if (!Helper.isNumber(sourceStr))
                    sourceStr = GlobalVars[sourceStr];

                GlobalVars[destinationVar] = destinationStr + sourceStr;

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));

            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }

        public void regexSplit(List<string> splt)
        {
            String methodName = "regexSplit";
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                String destinationList = splt[2];
                String destinationString = splt[3];
                String regexPattern = splt[4];

                
                
                if (Helper.isString(destinationString))
                    destinationString = Helper.extractStringContent(destinationString);
                else if(!Helper.isNumber(destinationString))
                    destinationString = GlobalVars[destinationString];

                if (Helper.isString(regexPattern))
                    regexPattern = Helper.extractStringContent(regexPattern);
                else if (!Helper.isNumber(regexPattern))
                    regexPattern = GlobalVars[regexPattern];


                List<String> splitList = Regex.Split(destinationString, regexPattern, RegexOptions.IgnoreCase).ToList();
                List<String> assignList = new List<string>();

                foreach(String str in splitList)
                {
                    assignList.Add(Helper.trasformToString(str));
                }

                GlobalList[destinationList] = assignList;
                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch(Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }

        public void findString(List<string> splt)
        {
            String methodName = "findString";
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));

                String destinationVar = splt[2];
                String strInput = splt[3];
                String inStr = String.Empty;

                if (Helper.isNumber(strInput))
                    inStr = strInput;
                else if (Helper.isString(strInput))
                    inStr = Helper.extractStringContent(strInput);
                else
                    inStr = GlobalVars[strInput];

                String searchString = splt[4];

                if (Helper.isString(searchString))
                    searchString = Helper.extractStringContent(searchString);
                else if (Helper.isNumber(searchString))
                    searchString = searchString.ToString();
                else
                    searchString = GlobalVars[searchString];



                int pos = inStr.IndexOf(searchString);
                GlobalVars[destinationVar] = pos.ToString();

                log.Info(String.Format("{0} - {1} END", this.GetType().Name, methodName));
            }
            catch(Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;

            }
        }


        public void shell(List<string> splt)
        {
            string methodName = nameof(shell);
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));


                string listname = splt[1];
                string command = splt[2];

            

                if (Helper.isString(command))
                    command = Helper.extractStringContent(command);
                else
                    command = this.GlobalVars[command];
               
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c \"" + command + "\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };
                proc.Start();

                List<String> newList = new List<string>();
           
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    newList.Add(line);
                }
                GlobalList[listname] = newList;
                GlobalVars["[EXITCODE]"] = proc.ExitCode.ToString();

            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
        public void execFunction(List<string> splt)
        {
            string methodName = "execFunction";
            try
            {
                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                Process process = new Process();

                String listame = splt[1];
                List<String> commandList = splt[2].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (commandList.Count >=1)
                {
                    process.StartInfo.FileName = commandList[0].Trim();
                    String args = "";
                    if (commandList.Count >= 2)
                    {
                        for (int i = 1; i < commandList.Count; i++)
                            args += commandList[i] + " ";
                    }
                    

                    var proc = new Process
                    {
                        
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = commandList[0].Trim(),
                            Arguments = args,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    List<String> newList = new List<string>();
                    proc.Start();
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        string line = proc.StandardOutput.ReadLine();
                        newList.Add(line);
                    }
                    GlobalList[listame] = newList;
                    GlobalVars["[EXITCODE]"] = proc.ExitCode.ToString();

                }
             

            }
            catch(Exception ex)
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
                        input = Helper.extractStringContent(input);
                        if (!endLine)
                            Console.Write(input);
                        else
                            Console.WriteLine(input);

                    }
                    else
                    {
                        if (this.GlobalVars.ContainsKey(input)) { 
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
                        else
                        {
                            List<string> lst = this.GlobalList[input];
                            string outPut;
                            foreach (String actString in lst)
                            {
                                if (Helper.isString(actString))
                                    outPut = Helper.extractStringContent(actString);
                                else
                                    outPut = actString;
                                if (!endLine)
                                    Console.Write(outPut);
                                else
                                    Console.WriteLine(outPut);
                            }
                        }
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
                if (Helper.isNumber(istruction[1]))
                    return Int32.Parse(istruction[1]);


                
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


                subAssegnamento(varAssegnamento, Helper.trasformToString(this.GlobalList[listName][inx]));


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

                if(!Helper.isNumber(tmpInput))
                    subAssegnamento(varName, Helper.trasformToString(tmpInput));
                else
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
