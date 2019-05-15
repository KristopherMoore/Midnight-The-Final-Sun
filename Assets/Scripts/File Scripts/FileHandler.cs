//Program Information///////////////////////////////////////////////////////////
/*
 * @file FileHandler.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Initial Build of FileHandler
 *          
 *          Responsible for handling all file operations of the game,
 *          reading, writing etc.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileHandler
{

    private string defaultPath = "Assets/Resources/";

    //given character data info, saves it to a file
    public void SaveGame(SaveInfo saveInfo)
    {
        string saveLocationPath = getPath(ref saveInfo);

        //open writer stream, using built in C# to check for existense of file, and clear anything in there
        StreamWriter writer = new StreamWriter(saveLocationPath, false);

        //write all relevant data
        //writing Unit Information //ORDER MUST BE CONSISTENT WITH READER
        writer.WriteLine(saveInfo.playerObject.getName());
        writer.WriteLine(saveInfo.playerObject.getHP());
        writer.WriteLine(saveInfo.playerObject.getMaxHP());
        writer.WriteLine(saveInfo.playerObject.getDamageResistance());
        writer.WriteLine(saveInfo.playerObject.getMaxDR());
        writer.WriteLine(saveInfo.playerObject.getStamina());
        writer.WriteLine(saveInfo.playerObject.getMaxStamina());

        //writing Player Information
        writer.WriteLine(saveInfo.playerObject.getSTR());
        writer.WriteLine(saveInfo.playerObject.getINT());
        writer.WriteLine(saveInfo.playerObject.getAGI());
        writer.WriteLine(saveInfo.playerObject.getEND());
        writer.WriteLine(saveInfo.playerObject.getFAI());
        writer.WriteLine(saveInfo.playerObject.getVIG());

        //TODO: SETUP INVENTORY LOADING AND EQUIPPED, will need a list of objectName strings.
        //writer.WriteLine(saveInfo.playerObject.getInventory());
        //writer.WriteLine(saveInfo.playerObject.getWeaponRight());
        //writer.WriteLine(saveInfo.playerObject.getWeaponLeft());
        //writer.WriteLine(saveInfo.playerObject.getArmorArms());
        //writer.WriteLine(saveInfo.playerObject.getArmorBody());
        //writer.WriteLine(saveInfo.playerObject.getArmorHelm());
        //writer.WriteLine(saveInfo.playerObject.getArmorLegs());

        Debug.Log("WRTIER FINISHED");
        writer.Close();
    }


    //given save info, deletes it from records
    public void DeleteGame(SaveInfo saveInfo)
    {
        string saveLocationPath = getPath(ref saveInfo);

        //check for file exisitence, if so delete it
        if (File.Exists(saveLocationPath))
        {
            File.Delete(saveLocationPath);
        }

    }

    //given save info, initiates load of records
    public Player LoadGame(SaveInfo saveInfo)
    {
        string saveLocationPath = getPath(ref saveInfo);
        Player playerObject = new Player();

        if (File.Exists(saveLocationPath))
        {
            StreamReader reader = new StreamReader(File.Open(saveLocationPath, System.IO.FileMode.Open));

            //read in saved player information //ORDER MUST BE CONSISTENT WITH WRITER
            //read in Unit information
            playerObject.setName(reader.ReadLine());
            playerObject.setHP(float.Parse(reader.ReadLine()));
            playerObject.setMaxHP(float.Parse(reader.ReadLine()));
            playerObject.setDamageResistance(float.Parse(reader.ReadLine()));
            playerObject.setMaxDR(float.Parse(reader.ReadLine()));
            playerObject.setStamina(float.Parse(reader.ReadLine()));
            playerObject.setMaxStamina(float.Parse(reader.ReadLine()));

            //read in Player information
            playerObject.setSTR(int.Parse(reader.ReadLine()));
            playerObject.setINT(int.Parse(reader.ReadLine()));
            playerObject.setAGI(int.Parse(reader.ReadLine()));
            playerObject.setEND(int.Parse(reader.ReadLine()));
            playerObject.setFAI(int.Parse(reader.ReadLine()));
            playerObject.setVIG(int.Parse(reader.ReadLine()));

            //TODO: SETUP LOADING OF INVENTORY AND EQUIPPED, LOOK ABOVE TO SAVER FIRST
            //playerObject.setInventory();
            //playerObject.setWeaponRight();
            //playerObject.setWeaponLeft();
            //playerObject.setArmorArms();
            //playerObject.setArmorBody();
            //playerObject.setArmorHelm();
            //playerObject.setArmorLegs();

            reader.Close();
        }

        //return constructed player to calling method
        return playerObject;
    }

    private string getPath(ref SaveInfo saveInfo)
    {
        string saveLocationPath = defaultPath;

        //check if empty, if so default name, else append onto saveLocationPath
        if (saveInfo.playerObject.getName() == "")
            saveLocationPath = saveLocationPath + "DEFAULT";
        else
            saveLocationPath = saveLocationPath + saveInfo.playerObject.getName();

        //add our file extension to the path
        saveLocationPath = saveLocationPath + ".midnight";

        return saveLocationPath;
    }

}
