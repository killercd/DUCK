using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duck.Net
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            bool replMode = false;
            DuckProgramControl program = new DuckProgramControl();

            if (args.Length < 1)
            {
                replMode = true;
                program.LineList = new List<string>();
                program.LineList.Add("");
            }

            else {
                string file = args[0];

                program.loadProgram(file);

            }


            Tokenizer scanner = new Tokenizer(program.LineList);

            program.GlobalList["[ARGV]"] = args.ToList();

            while (replMode || (program.EIP < program.LineList.Count && program.EIP >= 0))
            {


                
                string lbl;
                string iteratorListName;
                //string procedureName;

               

                //scanner.nextLine();
                //Token token = scanner.getNextToken(LexerAction.CONSUME_TOKEN);
                //if (token.Name == TokenName.SET)
                //{
                //    program.catchAssignment(scanner, token);
                //    program.EIP++;
                //    continue;
                //}
                if (replMode) {
                    Console.Write("DUCK> ");
                    program.EIP = 0;
                    program.LineList[program.EIP] = Console.ReadLine();
                }
                lbl = program.catchLabel(program.LineList[program.EIP]);
                if (!String.IsNullOrEmpty(lbl))
                {
                    program.EIP++;
                    continue;
                }

                iteratorListName = program.catchIterator(program.LineList[program.EIP]);
                if (!String.IsNullOrEmpty(iteratorListName))
                {
                    program.EIP++;
                    program.enableIteration(iteratorListName);
                    continue;
                }
                if (program.catchEndIterator(program.LineList[program.EIP]))
                {
                    program.checkIterationEnd();
                    continue;
                }

                //if (program.catchProcedure(program.LineList[program.EIP]))
                //{
                //    program.gotoEndProcedure();
                //    continue;
                //}
                //Express mode
                if(program.LineList[program.EIP].StartsWith("@"))
                {
                    program.printObject(program.LineList[program.EIP]);
                    program.EIP++;
                    continue;
                }
                //Extract action
                String action = "";
                String ln = "";
                if (program.LineList[program.EIP].Length > 0) {
                    ln = program.LineList[program.EIP];
                    ln = ln.TrimStart();
                    action = ln.Substring(0, ln.IndexOf(' '));
                    ln = ln.Substring(ln.IndexOf(' ') + 1);
                    ln = ln.TrimStart();

                }
                else
                {
                    program.EIP++;
                    continue;
                }

                List<String> actionList = new List<string>();
                actionList.Add("NOP");
                actionList.AddRange(ln.Split('_').ToList());


                if (actionList.Count() <= 1)
                {
                    program.EIP++;
                    continue;
                }

                

                if (action == "prnt")
                {
                    program.subPrint(ln, false);
                    program.EIP++;
                }
                else if (action == "print")
                {
                    program.subPrint(ln, false);
                    program.EIP++;
                }
                else if (action == "printnl")
                {
                    program.subPrint(ln, true);
                    program.EIP++;
                }
                else if (action == "prntnl")
                {
                    program.subPrint(ln, true);
                    program.EIP++;
                }
                else if (action == "goto")
                {
                    program.EIP = program.subGoto(actionList);
                }
                else if (action == "==")
                {
                    bool ipChanged = false;
                    program.EIP = program.compareFunction(actionList, ref ipChanged, "==");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (action == "!=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(actionList, ref ipChanged, "!=");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (action == ">")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(actionList, ref ipChanged, ">");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (action == ">=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(actionList, ref ipChanged, ">=");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (action == "<")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(actionList, ref ipChanged, "<");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (action == "<=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(actionList, ref ipChanged, "<=");
                    if (!ipChanged)
                        program.EIP++;
                }

                else if (action == "set")
                {
                    program.subAssegnamento(ln);
                    program.EIP++;
                }
                else if (action == "+")
                {
                    program.subOper(actionList, '+');
                    program.EIP++;
                }
                else if (action == "-")
                {
                    program.subOper(actionList, '-');
                    program.EIP++;
                }
                else if (action == "*")
                {
                    program.subOper(actionList, '*');
                    program.EIP++;
                }
                else if (action == "/")
                {
                    program.subOper(actionList, '/');
                    program.EIP++;
                }
                else if (action == "%")
                {
                    program.subOper(actionList, '%');
                    program.EIP++;
                }
                else if (action == "die")
                {
                    program.subExit(actionList);
                    program.EIP++;
                }
                else if (action == "list")
                {
                    if (actionList[1] == "create")
                    {
                        program.createList(actionList);
                    }
                    else if (actionList[1] == "size")
                    {
                        program.listSize(actionList);
                    }
                    else if (actionList[1] == "read")
                    {
                        program.listRead(actionList);
                    }
                    else if (actionList[1] == "write")
                    {
                        program.listWrite(actionList);
                    }
                    else if (actionList[1] == "add")
                    {
                        program.listAdd(actionList);
                    }
                    program.EIP++;
                }
                else if (action == "input")
                {
                    if (actionList[1] == "key")
                    {
                        program.readFromKeyboad(actionList);
                    }
                    else if (actionList[1] == "textfile")
                    {
                        //                if(splt[2]=="open"){
                        //                    openFileText(splt, globalVars);
                        //                }else if(splt[2]=="close"){
                        //                    closeFileText(splt, globalVars);
                        //                }
                        program.loadTextFile(actionList);
                    }
                    program.EIP++;
                }
                else if (action == "iterator")
                {
                    program.iteratorFunction(actionList);

                }
                else if (action == "exec")
                {
                    program.execFunction(actionList);
                    program.EIP++;

                }
                else if(action == "$$")
                {
                    program.shell(actionList);
                    program.EIP++;

                }
              

                else if (action == "str")
                {
                    if (actionList[1] == "find") {
                        program.findString(actionList);
                    }
                    else if (actionList[1] == "regexsplit")
                    {
                        program.regexSplit(actionList);
                    }
                    else if (actionList[1] == "append")
                    {
                        program.stringAppend(actionList);
                    }
                    else if (actionList[1] == "tolist")
                    {
                        program.stringToList(actionList);
                    }
                    program.EIP++;

                }
                else
                {

                    program.EIP++;
                }

            }
        }

        private static void usage(string[] args)
        {
            try
            {
                Console.WriteLine("duck.exe program.dck");

            }
            catch (Exception ex)
            {
                log.Error("Si è verificato un errore usage()", ex);
            }
        }
    }
}
