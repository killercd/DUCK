
std::vector<std::string> split(std::string strToSplit, char delimeter)
{
    std::stringstream ss(strToSplit);
    std::string item;
    std::vector<std::string> splittedStrings;
    while (std::getline(ss, item, delimeter))
    {
       splittedStrings.push_back(item);
    }
    return splittedStrings;
}
bool isString(std::string str){
    if(str.size()>1 && str[0] == '"')
        return true;
    return false;
}

std::string extractStringContent(std::string str){
    return str.substr(1, str.size()-2);
}

bool isNumber(std::string str){
    if(str.compare("0")==0)
        return true;
    if(atol(str.c_str())>0)
        return true;
    else
        return false;
}


//void printDebugMap(std::map<std::string, std::string> myMap){
//    std::cout << "GLOBAL VAAAAR" << std::endl;
//
//
//
//    std::cout << "END OF GLOBAL VAAAAR" << std::endl;;
//
//    return;
//}

std::string getString(char x)
{
    // string class has a constructor
    // that allows us to specify size of
    // string as first parameter and character
    // to be filled in given size as second
    // parameter.
    std::string s(1, x);

    return s;
}
