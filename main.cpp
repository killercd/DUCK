#include <iostream>
#include <fstream>
#include <string>
#include <bits/stdc++.h>
#include <cstddef>
#include "DuckStandard.h"
using namespace std;


void loadProgram(string fileName, vector<string> &lineList, map<string,int> &labelList);
void usage();
int compareFunction(vector<string> &lineIstruction, map<string,int> &labelList, map<string, string> &globalVars, int EIP, bool &IPIsChanged, string compareType);
void subAssegnamento(string destinazione, string sorgente, map<string, string> &globalVars);
void subAssegnamentoList(string destinazione, string sorgente,int index,map<string, string> &globalVars, map<string, vector<string> > &globalList);
void subPrint(vector<string> &lineIstruction, bool endLine, map<string, string> &globalVars);
bool catchLabel(string istruction, string &labelName);
int subGoto(vector<string> &istruction, map<string, int> &labelList);
void subOper(vector<string> &lineIstruction, map<string, string> &globalVars, char oper);
void createList(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList);
void listSize(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList);
void listRead(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList);
void listWrite(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList);
void listAdd(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList);
void readFromKeyboad(vector<string> &lineIstruction, map<string, string> &globalVars);
void openFileText(vector<string> &lineIstruction, map<string, string> &globalVars);
void closeFileText(vector<string> &lineIstruction, map<string, string> &globalVars);
void subExit(vector<string> &lineIstruction);
void loadTextFile(vector<string> &lineIstruction, map<string, vector<string> > &globalList);
void iteratorFunction(vector<string> &lineIstruction, map<string,int> &labelList, map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP);
bool catchIterator(string istruction, string &listName);
void enableIteration(string iteratorListName, map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP);
void checkIterationEnd( map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP);
bool catchEndIterator(string istruction);

int main(int argc, char *argv[])
{

    if(argc<1){
        usage();
        return 0;
    }

    int EIP = 0;

    string file = "hello.dck";//argv[1];
    vector<string> lineList;
    map<string, string> globalVars;
    map<string, vector<string> > globalList;
    map<string, int> labelList;
    loadProgram(file, lineList, labelList);


    while(EIP<lineList.size() && EIP >= 0){

        //globalVars["[ITERLIST]"] = lineIstruction[1];
//        globalVars["[ITERSTART]"] = labelList[lineIstruction[2]];
//        globalVars["[ITEREND]"] = labelList[lineIstruction[3]];
//        globalVars["[INLOOPING]"] = 1;
//        globalVars["[ITERCOUNTER]"] = 0;


        string lbl;
        string iteratorListName;
        vector<string> splt;


        if(catchLabel(lineList[EIP], lbl)){
            EIP++;
            continue;
        }

        if(catchIterator(lineList[EIP], iteratorListName)){
            EIP++;

            enableIteration(iteratorListName, globalVars, globalList,  EIP);
            continue;
        }
        if(catchEndIterator(lineList[EIP])){
            checkIterationEnd(globalVars, globalList,  EIP);
            continue;
        }
        splt = split(lineList[EIP], '_');

        if(splt.size()<=0){
            EIP++;
            continue;
        }

        else if(splt[0]=="prnt"){
            subPrint(splt, false, globalVars);
            EIP++;
        }
        else if(splt[0]=="prntnl"){
            subPrint(splt, true, globalVars);
            EIP++;
        }
        else if(splt[0]=="goto"){
            EIP = subGoto(splt, labelList);
        }
        else if(splt[0]=="=="){
            bool ipChanged = false;
            bool invertCompare = false;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, "==");
            if(!ipChanged)
                EIP++;
        }
        else if(splt[0]=="!="){
            bool ipChanged = false;
            bool invertCompare = true;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, "!=");
            if(!ipChanged)
                EIP++;
        }
        else if(splt[0]==">"){
            bool ipChanged = false;
            bool invertCompare = true;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, ">");
            if(!ipChanged)
                EIP++;
        }
         else if(splt[0]==">="){
            bool ipChanged = false;
            bool invertCompare = true;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, ">=");
            if(!ipChanged)
                EIP++;
        }
         else if(splt[0]=="<"){
            bool ipChanged = false;
            bool invertCompare = true;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, "<");
            if(!ipChanged)
                EIP++;
        }
         else if(splt[0]=="<="){
            bool ipChanged = false;
            bool invertCompare = true;
            EIP = compareFunction(splt, labelList, globalVars, EIP, ipChanged, "<=");
            if(!ipChanged)
                EIP++;
        }

        else if(splt[0]=="="){
            subAssegnamento( splt[1],splt[2], globalVars );
            EIP++;
        }
        else if(splt[0]=="+"){
            subOper( splt, globalVars,'+' );
            EIP++;
        }
        else if(splt[0]=="-"){
            subOper( splt, globalVars,'-' );
            EIP++;
        }
         else if(splt[0]=="*"){
            subOper( splt, globalVars,'*' );
            EIP++;
        }
         else if(splt[0]=="/"){
            subOper( splt, globalVars,'/' );
            EIP++;
        }
         else if(splt[0]=="%"){
            subOper( splt, globalVars,'%' );
            EIP++;
        }
         else if(splt[0]=="die"){
            subExit( splt);
            EIP++;
        }
         else if(splt[0]=="list"){
            if(splt[1]=="create"){
                createList(splt, globalVars, globalList);
            }
            else if(splt[1]=="size"){
                listSize(splt, globalVars, globalList);
            }
            else if(splt[1]=="read"){
                listRead(splt, globalVars, globalList);
            }
             else if(splt[1]=="write"){
                listWrite(splt, globalVars, globalList);
            }
            else if(splt[1]=="add"){
                listAdd(splt, globalVars, globalList);
            }
            EIP++;
        }else if(splt[0]=="input"){
            if(splt[1]=="key"){
                readFromKeyboad(splt, globalVars);
            }
            else if(splt[1]=="textfile"){
//                if(splt[2]=="open"){
//                    openFileText(splt, globalVars);
//                }else if(splt[2]=="close"){
//                    closeFileText(splt, globalVars);
//                }
                loadTextFile(splt, globalList);
            }
            EIP++;
        }
        else if(splt[0]=="iterator"){
            iteratorFunction(splt, labelList, globalVars, globalList, EIP);

        }
        else{

            EIP++;
        }

    }

    return 0;
}


void loadProgram(string fileName, vector<string> &lineList, map<string,int> &labelList){
    fstream in;
    string s;

    in.open(fileName.c_str(), std::fstream::in | std::fstream::out | std::fstream::app);

    if(!in.is_open()){
        cout << "File Non Trovato ARGH!" << endl;
        exit(-1);
    }
    string tmpLine;
    string labelName;
    int counter = 0;
    while(getline(in, tmpLine )) {
        if(catchLabel(tmpLine, labelName))
            labelList[labelName] = counter;

        lineList.push_back(tmpLine);
        counter++;
    }
    in.close();


}

int compareFunction(vector<string> &lineIstruction, map<string,int> &labelList, map<string, string> &globalVars, int EIP, bool &IPIsChanged, string compareType){
    try{

        string term1 = lineIstruction[1];
        string term2 = lineIstruction[2];
        string jumpLabel = lineIstruction[3];




        if(isString(term1))
            term1 = extractStringContent(term1);
        else
            if(!isNumber(term1))
                term1 = globalVars[term1];

        if(isString(term2))
            term2 = extractStringContent(term2);
        else
            if(!isNumber(term2))
                term2 = globalVars[term2];

        int jumpTo = 0;

        if(!isNumber(jumpLabel))
            jumpTo = labelList[jumpLabel];
        else
            jumpTo = atoi(jumpLabel.c_str());



        if(
           (compareType.compare("==")==0 && (term1.compare(term2)==0)) ||
           (compareType.compare("!=")==0 && (term1.compare(term2)!=0)) ||
            (compareType.compare(">")==0 && (atol(term1.c_str()) > atol(term2.c_str()) )) ||
            (compareType.compare(">=")==0 && (atol(term1.c_str()) >= atol(term2.c_str()) )) ||
           (compareType.compare("<")==0 && (atol(term1.c_str()) < atol(term2.c_str()) )) ||
           (compareType.compare("<=")==0 && (atol(term1.c_str()) < atol(term2.c_str()) ))
           ){
            EIP = jumpTo;
            IPIsChanged = true;
        }




        IPIsChanged=false;
        return EIP;


    }catch(int err){
        cout << "Error: invalid syntax on compare function" << endl;
        exit(-1);
    }

}

void subPrint(vector<string> &lineIstruction, bool endLine, map<string, string> &globalVars){
    try{
        if(lineIstruction.size()>1){
            string input = lineIstruction[1];
            if(isString(input)){ //start of string match

                if(!endLine)
                    cout << extractStringContent(input);
                else
                    cout << extractStringContent(input) << endl;

            }else{
                input = globalVars[input];
                string outPut;
                if(isString(input))
                    outPut = extractStringContent(input);
                else
                    outPut = input;
                if(!endLine)
                    cout << outPut;
                else
                    cout << outPut << endl;
            }

        }

    }catch(int err){
        cout << "Error: invalid syntax on print function" << endl;
        exit(-1);
    }
}

void subAssegnamento(string destinazione, string sorgente, map<string, string> &globalVars){

    if(isString(sorgente)){
        globalVars[destinazione] = extractStringContent(sorgente);
    }else if(isNumber(sorgente)){
        globalVars[destinazione] = sorgente;
    }
    else
        globalVars[destinazione] = globalVars[sorgente];

}
void subAssegnamentoList(string destinazione, string sorgente,int index, map<string, string> &globalVars,  map<string, vector<string> > &globalList){
    if(isString(sorgente)){
        if(index==-1)
            globalList[destinazione].push_back(extractStringContent(sorgente));
        else
            globalList[destinazione][index] = extractStringContent(sorgente);
    }else if(isNumber(sorgente)){
        if(index==-1)
            globalList[destinazione].push_back(sorgente);
        else
            globalList[destinazione][index] = sorgente;
    }
    else
        if(index==-1)
            globalList[destinazione].push_back(globalVars[sorgente]);
        else
            globalList[destinazione][index] = globalVars[sorgente];
}


void usage(){
    cout << "USAGE: Duck file.dck" << endl;
}

bool catchLabel(string istruction, string &labelName){
    if(istruction.size()>1 &&
        istruction[0]==':'){
        labelName = istruction.substr(1);

        return true;
    }
    return false;

}

int subGoto(vector<string> &istruction, map<string, int> &labelList){
    int line = atoi(istruction[1].c_str());
    if(line > 0)
        return line;

    return labelList[istruction[1]];

}

void subOper(vector<string> &lineIstruction, map<string, string> &globalVars, char oper){

        string varName = lineIstruction[1];
        string term1 = lineIstruction[2];
        string term2 = lineIstruction[3];

        long mTerm1=0;
        long mTerm2=0;
        long result=0;
        if(isNumber(term1))
            mTerm1 = atol(term1.c_str());
        else
            mTerm1 = atol(globalVars[term1].c_str());

        if(isNumber(term2))
            mTerm2 = atol(term2.c_str());
        else
            mTerm2 = atol(globalVars[term2].c_str());

        if(oper=='+')
            result = mTerm1 + mTerm2;
        else if(oper=='-')
            result = mTerm1 - mTerm2;
        else if(oper=='*')
            result = mTerm1 * mTerm2;
        else if(oper=='/')
            result = mTerm1 / mTerm2;
        else if(oper=='%')
            result = mTerm1 % mTerm2;

        stringstream mystream;
        mystream << result;

        subAssegnamento(varName, mystream.str(), globalVars);

}

void createList(vector<string> &lineIstruction,
                map<string, string> &globalVars,
                map<string, vector<string> > &globalList){

    vector<string> spltTerm;
    if(lineIstruction.size()>3)
        spltTerm = split(lineIstruction[3], ',');
    string listName = lineIstruction[2];

    vector<string> newList;

    for(int i=0; i<spltTerm.size(); i++){
        if(isNumber(spltTerm[i]))
            newList.push_back(spltTerm[i]);
        else if(isString(spltTerm[i]))
            newList.push_back(extractStringContent(spltTerm[i]));
        else
            newList.push_back(globalVars[spltTerm[i]]);


    }
    globalList[listName] = newList;

}

void listSize(vector<string> &lineIstruction,
              map<string, string> &globalVars,
              map<string, vector<string> > &globalList){
    stringstream mystream;
    mystream << globalList[lineIstruction[3]].size();
    subAssegnamento(lineIstruction[2], mystream.str(), globalVars);
}

void listRead(vector<string> &lineIstruction,
              map<string, string> &globalVars,
              map<string, vector<string> > &globalList){

    string varAssegnamento = lineIstruction[2];
    string listName = lineIstruction[3];
    string listIndex = lineIstruction[4];


    long inx = 0;
    if(!isNumber(listIndex))
        inx = atol(globalVars[listIndex].c_str());
    else
        inx = atol(listIndex.c_str());


    subAssegnamento(varAssegnamento, globalList[listName][inx],globalVars);

}

void listWrite(vector<string> &lineIstruction,
               map<string, string> &globalVars,
               map<string, vector<string> > &globalList){

    string listName = lineIstruction[2];
    string listIndex = lineIstruction[3];
    string value = lineIstruction[4];


    subAssegnamentoList(listName, value, atoi(listIndex.c_str()), globalVars, globalList);

}
void listAdd(vector<string> &lineIstruction, map<string, string> &globalVars, map<string, vector<string> > &globalList){
    string listName = lineIstruction[2];
    string value = lineIstruction[3];

    subAssegnamentoList(listName, value, -1, globalVars, globalList);
}




void subExit(vector<string> &lineIstruction){
    if(lineIstruction.size()>1){
        exit(atoi(lineIstruction[1].c_str()));
    }
    else{
        exit(0);
    }
}

void readFromKeyboad(vector<string> &lineIstruction, map<string, string> &globalVars){
    string varName = lineIstruction[2];
    string tmpInput;

    getline(cin, tmpInput);

    subAssegnamento(varName, tmpInput,globalVars);
}
void openFileText(vector<string> &lineIstruction, map<string, string> &globalVars){
    string varName = lineIstruction[3];
    string fileName = lineIstruction[4];
    string flag = lineIstruction[5];
    if(flag=="R"){
        ifstream in(fileName.c_str());
        if(!in) {
            cout << "Cannot open input " << fileName << endl;
            exit(-1);
        }

        stringstream mystream;
        mystream << &in;
        globalVars[varName] = mystream.str();

    }

}
void closeFileText(vector<string> &lineIstruction, map<string, string> &globalVars){
     string varName = lineIstruction[3];
     ifstream *in = (ifstream *)atol(globalVars[varName].c_str());
     in->close();

}
void loadTextFile(vector<string> &lineIstruction, map<string, vector<string> > &globalList){

    string listName = lineIstruction[2];
    string path = lineIstruction[3];
    vector<string> lineList;

    ifstream File;

    ifstream* f_ptr = &File;//want to get the basics right before trying smart pointers;
    f_ptr->open(path.c_str());// end result is the same even if I use path.c_str() instead;
    if(f_ptr->is_open()){
        while(!f_ptr->eof()){
            string line;
            string app = "";
            getline(*f_ptr, line);
            app.append("\"");
            app.append(line);
            app.append("\"");
            lineList.push_back(app);
        }

        f_ptr->close();
    }

    globalList[listName] = lineList;
    delete f_ptr;


}

void iteratorFunction(vector<string> &lineIstruction, map<string,int> &labelList, map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP){
    globalVars["[ITERLIST]"] = lineIstruction[1];
    globalVars["[ITERSTART]"] = labelList[lineIstruction[2]];
    globalVars["[INLOOPING]"] = "1";
    globalVars["[ITERCOUNTER]"] = "0";
    EIP = labelList[lineIstruction[2]];

}

bool catchIterator(string istruction, string &listName){
    int pos = 0;
    int listNameStart = 0;
    pos = istruction.find("[ITERATOR ");

    if(pos>=0){
        listNameStart = 10;
        int pointer = listNameStart;
        char c = istruction[listNameStart];
        while(c!=']'){
            listName.append(getString(c));
            pointer++;
            c = istruction[pointer];
        }
        return true;
    }
    return false;
}
bool catchEndIterator(string istruction){
    if(istruction=="[END ITERATOR]")
        return true;
    else
        return false;
}
void enableIteration(string iteratorListName, map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP){

    int listSize = globalList[iteratorListName].size();
    stringstream eipStream;
    eipStream << EIP;

    stringstream listSizeStream;
    listSizeStream << listSize;

//    cout << iteratorListName << endl;
//    cout << eipStream.str() << endl;
//    cout << listSizeStream.str() << endl;


    globalVars["[ITERLIST]"] = iteratorListName;
    globalVars["[ITERSTART]"] = eipStream.str();

    globalVars["[ITERCOUNTER]"] = "0";
    globalVars["[ITERLISTSIZE]"] = listSizeStream.str();

    if(listSize<=0){
        globalVars["[CURRENT]"] = "";
    }else{
        globalVars["[CURRENT]"] = globalList[iteratorListName][0];
    }

}
void checkIterationEnd(map<string, string> &globalVars,map<string, vector<string> > &globalList, int &EIP){
    string iteratorListName = globalVars["[ITERLIST]"];
    int listSize = globalList[iteratorListName].size();
    int actualIterCounter = atoi(globalVars["[ITERCOUNTER]"].c_str());

    actualIterCounter++;
    if(actualIterCounter<listSize){


        stringstream actualCounter;
        actualCounter << actualIterCounter;
        globalVars["[ITERCOUNTER]"] = actualCounter.str();
        globalVars["[CURRENT]"] = globalList[iteratorListName][actualIterCounter];

        EIP = atoi(globalVars["[ITERSTART]"].c_str());

    }else{
        EIP ++;
    }


}
