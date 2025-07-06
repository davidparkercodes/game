using Godot;
using Game.Application.Game.Services;
using Game.Infrastructure.Game;

namespace Game.Presentation.UI;

public partial class SpeedControl : CanvasLayer
{
    [Export] public Button? Speed1xButton;
    [Export] public Button? Speed2xButton;
    [Export] public Button? Speed4xButton;

    private ITimeManager? _timeManager;
    private const string LogPrefix = "⚡ [SPEED-CONTROL]";
    
    public override void _Ready()
    {
        // Defer initialization to ensure all child nodes are ready
        CallDeferred(nameof(DeferredInitialize));
    }
    
    private void DeferredInitialize()
    {
        InitializeNodeReferences();
        InitializeEventConnections();
        SetupInitialState();
        
        // Connect to TimeManager for speed change events
        ConnectToTimeManager();
    }

    private void InitializeNodeReferences()
    {
        // Use correct paths based on the fixed scene structure
        Speed1xButton = GetNodeOrNull<Button>("Panel/VBoxContainer/Speed1xButton");
        Speed2xButton = GetNodeOrNull<Button>("Panel/VBoxContainer/Speed2xButton");
        Speed4xButton = GetNodeOrNull<Button>("Panel/VBoxContainer/Speed4xButton");
        
        // Try FindChild for any missing buttons
        if (Speed1xButton == null || Speed2xButton == null || Speed4xButton == null)
        {
            if (Speed1xButton == null) Speed1xButton = FindChild("Speed1xButton", true, false) as Button;
            if (Speed2xButton == null) Speed2xButton = FindChild("Speed2xButton", true, false) as Button;
            if (Speed4xButton == null) Speed4xButton = FindChild("Speed4xButton", true, false) as Button;
        }
        
        // Final fallback: direct child traversal for any still missing buttons
        if (Speed1xButton == null || Speed2xButton == null || Speed4xButton == null)
        {
            var panel = GetNode("Panel");
            if (panel != null)
            {
                var vbox = panel.GetNode("VBoxContainer");
                if (vbox != null)
                {
                    if (Speed1xButton == null) Speed1xButton = vbox.GetNode("Speed1xButton") as Button;
                    if (Speed2xButton == null) Speed2xButton = vbox.GetNode("Speed2xButton") as Button;
                    if (Speed4xButton == null) Speed4xButton = vbox.GetNode("Speed4xButton") as Button;
                }
            }
        }
    }
    

    private void InitializeEventConnections()
    {
        if (Speed1xButton != null)
        {
            Speed1xButton.Pressed += OnSpeed1xPressed;
        }

        if (Speed2xButton != null)
        {
            Speed2xButton.Pressed += OnSpeed2xPressed;
        }

        if (Speed4xButton != null)
        {
            Speed4xButton.Pressed += OnSpeed4xPressed;
        }
    }

    private void SetupInitialState()
    {
        // Set initial button states (1x active by default)
        UpdateButtonStates(0); // 0 = 1x speed
    }

    private void ConnectToTimeManager()
    {
        // Use GodotTimeManager singleton instance
        _timeManager = GodotTimeManager.Instance;
        if (_timeManager != null)
        {
            _timeManager.SpeedChanged += OnSpeedChanged;
            
            // Update button states to match current speed
            UpdateButtonStates(_timeManager.CurrentSpeedIndex);
            GD.Print($"{LogPrefix} Connected to GodotTimeManager singleton");
        }
        else
        {
            GD.PrintErr($"{LogPrefix} GodotTimeManager.Instance is null - speed controls will not work");
        }
    }


    private void OnSpeed1xPressed()
    {
        _timeManager?.SetSpeedTo1x();
    }

    private void OnSpeed2xPressed()
    {
        _timeManager?.SetSpeedTo2x();
    }

    private void OnSpeed4xPressed()
    {
        _timeManager?.SetSpeedTo4x();
    }

    private void OnSpeedChanged(float newSpeed, int speedIndex)
    {
        UpdateButtonStates(speedIndex);
    }

    private void UpdateButtonStates(int activeSpeedIndex)
    {
        // Reset all buttons to normal state
        SetButtonState(Speed1xButton, false);
        SetButtonState(Speed2xButton, false);
        SetButtonState(Speed4xButton, false);

        // Set the active button
        switch (activeSpeedIndex)
        {
            case 0:
                SetButtonState(Speed1xButton, true);
                break;
            case 1:
                SetButtonState(Speed2xButton, true);
                break;
            case 2:
                SetButtonState(Speed4xButton, true);
                break;
        }
    }

    private void SetButtonState(Button? button, bool isActive)
    {
        if (button == null) return;

        // Set button as pressed/unpressed for visual feedback
        button.ButtonPressed = isActive;
        
        // Update button modulation for additional visual feedback
        if (isActive)
        {
            button.Modulate = Colors.White; // Bright when active
        }
        else
        {
            button.Modulate = new Color(0.8f, 0.8f, 0.8f, 1.0f); // Slightly dimmed when inactive
        }
    }

    public override void _ExitTree()
    {
        // Disconnect from TimeManager events
        if (_timeManager != null)
        {
            _timeManager.SpeedChanged -= OnSpeedChanged;
        }
    }
}
