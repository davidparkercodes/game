using Godot;
using Game.Application.Game.Services;

namespace Game.Presentation.UI;

public partial class SpeedControl : CanvasLayer
{
    [Export] public Button? Speed1xButton;
    [Export] public Button? Speed2xButton;
    [Export] public Button? Speed4xButton;
    [Export] public Button? Speed10xButton;
    [Export] public Button? Speed20xButton;

    private TimeManager? _timeManager;
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
        Speed10xButton = GetNodeOrNull<Button>("Panel/VBoxContainer/Speed10xButton");
        Speed20xButton = GetNodeOrNull<Button>("Panel/VBoxContainer/Speed20xButton");
        
        // Try FindChild for any missing buttons
        if (Speed1xButton == null || Speed2xButton == null || Speed4xButton == null || Speed10xButton == null || Speed20xButton == null)
        {
            if (Speed1xButton == null) Speed1xButton = FindChild("Speed1xButton", true, false) as Button;
            if (Speed2xButton == null) Speed2xButton = FindChild("Speed2xButton", true, false) as Button;
            if (Speed4xButton == null) Speed4xButton = FindChild("Speed4xButton", true, false) as Button;
            if (Speed10xButton == null) Speed10xButton = FindChild("Speed10xButton", true, false) as Button;
            if (Speed20xButton == null) Speed20xButton = FindChild("Speed20xButton", true, false) as Button;
        }
        
        // Final fallback: direct child traversal for any still missing buttons
        if (Speed1xButton == null || Speed2xButton == null || Speed4xButton == null || Speed10xButton == null || Speed20xButton == null)
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
                    if (Speed10xButton == null) Speed10xButton = vbox.GetNode("Speed10xButton") as Button;
                    if (Speed20xButton == null) Speed20xButton = vbox.GetNode("Speed20xButton") as Button;
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

        if (Speed10xButton != null)
        {
            Speed10xButton.Pressed += OnSpeed10xPressed;
        }

        if (Speed20xButton != null)
        {
            Speed20xButton.Pressed += OnSpeed20xPressed;
        }
    }

    private void SetupInitialState()
    {
        // Set initial button states (1x active by default)
        UpdateButtonStates(0); // 0 = 1x speed
    }

    private void ConnectToTimeManager()
    {
        _timeManager = TimeManager.Instance;
        if (_timeManager != null)
        {
            _timeManager.SpeedChanged += OnSpeedChanged;
            
            // Update button states to match current speed
            UpdateButtonStates(_timeManager.CurrentSpeedIndex);
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

    private void OnSpeed10xPressed()
    {
        _timeManager?.SetSpeedTo10x();
    }

    private void OnSpeed20xPressed()
    {
        _timeManager?.SetSpeedTo20x();
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
        SetButtonState(Speed10xButton, false);
        SetButtonState(Speed20xButton, false);

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
            case 3:
                SetButtonState(Speed10xButton, true);
                break;
            case 4:
                SetButtonState(Speed20xButton, true);
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
