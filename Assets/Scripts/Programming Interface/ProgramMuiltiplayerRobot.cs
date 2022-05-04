using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class ProgramMuiltiplayerRobot : MonoBehaviour
{
    Queue<Instructions> instructionsQueue = new Queue<Instructions>();

    public Slider gear;
    private float gearValue;
    public TextMeshProUGUI textInstructions;

    public Button removeButton;
    public Button runButton;
    public Button stopButton;
    public Slider energySlider;
    float energyValue = 100f;

    public Button moveForwardButton;
    string moveForwardString = "Move Forward ";

    public Button moveBackwardButton;
    string moveBackwardString = "Move Backward ";

    public Button turnRightButton;
    string turnRightString = "Turn Right ";

    public Button turnLeftButton;
    string turnLeftString = "Turn Left ";

    public RobotMultiplayerInstructionScript instructionScript;
    public GameObject robot;
    RobotEnergy energyScript;
    
    public int movingEnergy = 5;


    void Start()
    {
        runButton = runButton.GetComponent<Button>();
        runButton.onClick.AddListener(sendProgramToRobot);

        stopButton = stopButton.GetComponent<Button>();
        stopButton.onClick.AddListener(stopProgram);

        removeButton = removeButton.GetComponent<Button>();
        removeButton.onClick.AddListener(removeLastInstruction);

        // moveForwardButton = moveForwardButton.GetComponent<Button>();
        // moveForwardButton.onClick.AddListener(addInstructionToProgram(moveForwardString, gearValue));

        moveForwardButton = moveForwardButton.GetComponent<Button>();
        moveForwardButton.onClick.AddListener(addMoveForwardToProgram);

        moveBackwardButton = moveBackwardButton.GetComponent<Button>();
        moveBackwardButton.onClick.AddListener(addMoveBackwardToProgram);

        turnRightButton = turnRightButton.GetComponent<Button>();
        turnRightButton.onClick.AddListener(addTurnRightToProgram);

        turnLeftButton = turnLeftButton.GetComponent<Button>();
        turnLeftButton.onClick.AddListener(addTurnLeftToProgram);

    }

    public void SetRobot(GameObject playerRobot)
    {
        robot = playerRobot;
        energyScript = robot.GetComponent<RobotEnergy>();
    }
    void Update()
    {
        gearValue = gear.GetComponent<Slider>().value;
        energySlider.value -= (energySlider.value - energyValue) * Time.deltaTime;
    }
    // void addInstructionToProgram(string instruction, float gearValue)
    // {
    //     textInstructions.text = textInstructions.text + instruction + gearValue + "\n";
    // }

    void SetEnergySlider()
    {
        energyValue = energyScript.energyPoints.Value;
    }
    void addMoveForwardToProgram()
    {
        if(energyScript.energyPoints.Value >= movingEnergy * gearValue){
            textInstructions.text = textInstructions.text + moveForwardString + gearValue + "\n";
            energyScript.useEnergy(movingEnergy*gearValue);
            SetEnergySlider();
        }
    }

    void addMoveBackwardToProgram()
    {
        if(energyScript.energyPoints.Value >= movingEnergy * gearValue){
            textInstructions.text = textInstructions.text + moveBackwardString + gearValue + "\n";
            energyScript.useEnergy(movingEnergy*gearValue);
            SetEnergySlider();

        }
    }

    void addTurnRightToProgram()
    {
        if(energyScript.energyPoints.Value >= movingEnergy * gearValue){
            textInstructions.text = textInstructions.text + turnRightString + gearValue + "\n";
            energyScript.useEnergy(movingEnergy*gearValue);
            SetEnergySlider();
        }
    }

    void addTurnLeftToProgram()
    {
        if(energyScript.energyPoints.Value >= movingEnergy * gearValue){
            textInstructions.text = textInstructions.text + turnLeftString + gearValue + "\n";
            energyScript.useEnergy(movingEnergy*gearValue);
            SetEnergySlider();

        }
    }

    void removeLastInstruction()
    {
        if(textInstructions.text == "") return;
        string[] seperateInstructions = textInstructions.text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();

        int gear = int.Parse(seperateInstructions[(seperateInstructions.Length - 1)].Split(' ')[2]);
        energyScript.restoreEnergy(gear*movingEnergy);
        SetEnergySlider();
        Debug.Log(energyScript.energyPoints.Value);
        
        seperateInstructions = seperateInstructions.SkipLast(1).ToArray();
        string instructionsString = String.Join("\n", seperateInstructions);
        textInstructions.text = instructionsString;
        if (textInstructions.text.Length != 0)
        {
            textInstructions.text = textInstructions.text + "\n";
        }
    }

    /// <summary>
    /// Takes string instruction and return the method for the instrucion for the queue
    /// </summary>
    private Instructions getInstruction(String subInstruction)
    {
        Instructions instruction = default;

        switch (subInstruction)
        {
            case "Forward":
                instruction = Instructions.MoveForward;
                break;
            case "Backward":
                instruction = Instructions.MoveBackward;
                break;
            case "Right":
                instruction = Instructions.RotateRight;
                break;
            case "Left":
                instruction = Instructions.RotateLeft;
                break;
            case "1":
                instruction = Instructions.FirstGear;
                break;
            case "2":
                instruction = Instructions.SecondGear;
                break;
            case "3":
                instruction = Instructions.ThirdGear;
                break;
        }
        return instruction;
    }

    /// <summary>
    /// Enqueue the instructions and load them into the robot and then executes the program
    /// </summary>
    private void sendProgramToRobot()
    {
        instructionsQueue.Clear();
        enqueueProgram();
        instructionScript.LoadInstructions(instructionsQueue);
        instructionScript.StartExecute();
    }

    /// <summary>
    /// Convert the program list into a queue of method instructions
    /// </summary>
    private void enqueueProgram()
    {
        String[] program = textInstructions.text.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        foreach (string instruction in program)
        {
            String[] subInstruction = instruction.Split(' ');
            instructionsQueue.Enqueue(getInstruction(subInstruction[2]));
            instructionsQueue.Enqueue(getInstruction(subInstruction[1]));
        }
    }

    private void stopProgram()
    {
        instructionScript.StopExecute();
        instructionsQueue.Clear();
    }


}