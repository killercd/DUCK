﻿using System;
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
            if (args.Length < 1)
            {
                usage(args);
                Environment.Exit(-1);
            }


            DuckProgramControl program = new DuckProgramControl();
            string file = args[0];

            program.loadProgram(file);


            while (program.EIP < program.LineList.Count && program.EIP >= 0)
            {

                //globalVars["[ITERLIST]"] = lineIstruction[1];
                //        globalVars["[ITERSTART]"] = labelList[lineIstruction[2]];
                //        globalVars["[ITEREND]"] = labelList[lineIstruction[3]];
                //        globalVars["[INLOOPING]"] = 1;
                //        globalVars["[ITERCOUNTER]"] = 0;


                string lbl;
                string iteratorListName;
                List<string> splt = new List<string>();

                lbl = program.catchLabel(program.LineList[program.EIP]);
                if (!String.IsNullOrEmpty(lbl))
                {
                    program.EIP++;
                    continue;
                }

                iteratorListName =  program.catchIterator(program.LineList[program.EIP]);
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
                splt = new List<string>(program.LineList[program.EIP].Split('_'));

                if (splt.Count() <= 0)
                {
                    program.EIP++;
                    continue;
                }

                else if (splt[0] == "prnt")
                {
                    program.subPrint(splt, false);
                    program.EIP++;
                }
                else if (splt[0] == "prntnl")
                {
                    program.subPrint(splt, true);
                    program.EIP++;
                }
                else if (splt[0] == "goto")
                {
                    program.EIP = program.subGoto(splt);
                }
                else if (splt[0] == "==")
                {
                    bool ipChanged = false;
                    program.EIP = program.compareFunction(splt, ref ipChanged, "==");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (splt[0] == "!=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(splt, ref ipChanged, "!=");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (splt[0] == ">")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(splt, ref ipChanged, ">");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (splt[0] == ">=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(splt, ref ipChanged, ">=");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (splt[0] == "<")
                {
                    bool ipChanged = false;
                  
                    program.EIP = program.compareFunction(splt, ref ipChanged, "<");
                    if (!ipChanged)
                        program.EIP++;
                }
                else if (splt[0] == "<=")
                {
                    bool ipChanged = false;

                    program.EIP = program.compareFunction(splt, ref ipChanged, "<=");
                    if (!ipChanged)
                        program.EIP++;
                }

                else if (splt[0] == "=")
                {
                    program.subAssegnamento(splt[1], splt[2]);
                    program.EIP++;
                }
                else if (splt[0] == "+")
                {
                    program.subOper(splt, '+');
                    program.EIP++;
                }
                else if (splt[0] == "-")
                {
                    program.subOper(splt, '-');
                    program.EIP++;
                }
                else if (splt[0] == "*")
                {
                    program.subOper(splt, '*');
                    program.EIP++;
                }
                else if (splt[0] == "/")
                {
                    program.subOper(splt, '/');
                    program.EIP++;
                }
                else if (splt[0] == "%")
                {
                    program.subOper(splt, '%');
                    program.EIP++;
                }
                else if (splt[0] == "die")
                {
                    program.subExit(splt);
                    program.EIP++;
                }
                else if (splt[0] == "list")
                {
                    if (splt[1] == "create")
                    {
                        program.createList(splt);
                    }
                    else if (splt[1] == "size")
                    {
                        program.listSize(splt);
                    }
                    else if (splt[1] == "read")
                    {
                        program.listRead(splt);
                    }
                    else if (splt[1] == "write")
                    {
                        program.listWrite(splt);
                    }
                    else if (splt[1] == "add")
                    {
                        program.listAdd(splt);
                    }
                    program.EIP++;
                }
                else if (splt[0] == "input")
                {
                    if (splt[1] == "key")
                    {
                        program.readFromKeyboad(splt);
                    }
                    else if (splt[1] == "textfile")
                    {
                        //                if(splt[2]=="open"){
                        //                    openFileText(splt, globalVars);
                        //                }else if(splt[2]=="close"){
                        //                    closeFileText(splt, globalVars);
                        //                }
                        program.loadTextFile(splt);
                    }
                    program.EIP++;
                }
                else if (splt[0] == "iterator")
                {
                    program.iteratorFunction(splt);

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
            catch(Exception ex)
            {
                log.Error("Si è verificato un errore usage()",ex);
            }
        }
    }
}