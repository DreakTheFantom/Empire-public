using System.Linq;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.Imperial.PlantsAnalyzer;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;
using Content.Shared.IdentityManagement;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Imperial.PlantsAnalyzer.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class PlantsAnalyzerWindow : FancyWindow
    {
        private readonly IEntityManager _entityManager;

        public PlantsAnalyzerWindow()
        {
            RobustXamlLoader.Load(this);

            var dependencies = IoCManager.Instance!;
            _entityManager = dependencies.Resolve<IEntityManager>();
        }

        public void Populate(PlantsAnalyzerScannedUserMessage msg)
        {
            var target = _entityManager.GetEntity(msg.TargetEntity);

            if (target == null)
            {
                NoPlantDataText.Visible = true;
                return;
            }

            NoPlantDataText.Visible = false;

            ScanModeLabel.Text = msg.ScanMode
                ? Loc.GetString("plants-analyzer-window-scan-mode-active")
                : Loc.GetString("plants-analyzer-window-scan-mode-inactive");
            ScanModeLabel.FontColorOverride = msg.ScanMode ? Color.Green : Color.Red;

            SpriteView.SetEntity(target.Value);
            SpriteView.Visible = msg.ScanMode;
            NoDataTex.Visible = !msg.ScanMode;

            var name = new FormattedMessage();
            name.PushColor(Color.White);
            name.AddText(Identity.Name(target.Value, _entityManager));
            NameLabel.SetMessage(name);

            SpeciesLabel.Text = msg.PlantName;

            PotencyLevelLabel.Text = $"{msg.PotencyLevel:F1}";
            ProductionLevelLabel.Text = $"{msg.ProductionLevel:F1} un";
            PestLevelLabel.Text = $"{msg.PestLevel * 10:F1} %";
            WeedLevelLabel.Text = $"{msg.WeedLevel * 10:F1} %";
            ToxinsLabel.Text = $"{msg.Toxins * 10:F1} %";
            AgeLabel.Text = $"{msg.Age}";
            HealthLabel.Text = $"{msg.Health:F1}";
            MutationLevelLabel.Text = $"{msg.MutationLevel:F1}";

            DeadLabel.Visible = msg.IsDead;
            DeadLabel.Text = Loc.GetString("plants-analyzer-window-plant-dead");

            MutationsLabel.Text = $"{Loc.GetString("plants-analyzer-window-mutations")} {msg.Mutations}";
            var chemicals = msg.Chemicals;
            if (chemicals?.Any() == true)
            {
                var chemicalsWithLineBreaks = chemicals;

                ChemicalsLabelWithLineBreaks.Text = $"{Loc.GetString("plants-analyzer-window-chemicals")} {chemicalsWithLineBreaks}";
            }
            else
            {
                ChemicalsLabelWithLineBreaks.Text = Loc.GetString("plants-analyzer-window-no-chemicals");
            }

            var showAlerts = !string.IsNullOrEmpty(msg.OptimalConditions) || msg.HasKudzu;

            AlertsDivider.Visible = showAlerts;
            AlertsContainer.Visible = showAlerts;

            if (showAlerts)
            {
                AlertsContainer.DisposeAllChildren();

                if (msg.HasKudzu == true)
                {
                    var kudzuAlertContainer = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Vertical,
                        HorizontalAlignment = HAlignment.Center,
                        VerticalAlignment = VAlignment.Center,
                        Margin = new Thickness(0, 4),
                    };

                    var kudzuIcon = new TextureRect
                    {
                        TexturePath = "/Textures/Objects/Devices/health_analyzer.rsi/toxin.png",
                        SetSize = new Vector2(15, 15),
                        HorizontalAlignment = HAlignment.Center,
                        VerticalAlignment = VAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10),
                    };

                    kudzuAlertContainer.AddChild(kudzuIcon);

                    var kudzuTextLabel = new RichTextLabel
                    {
                        Margin = new Thickness(0,10, 0, 0),
                        MaxWidth = 400,
                    };

                    kudzuTextLabel.SetMessage(Loc.GetString("plants-analyzer-window-kudzu-warning"), defaultColor: Color.Red);

                    kudzuAlertContainer.AddChild(kudzuTextLabel);

                    AlertsContainer.AddChild(kudzuAlertContainer);

                    AlertsContainer.AddChild(new PanelContainer
                    {
                        StyleClasses = { "LowDivider" },
                        Visible = true
                    });
                }

                if (!string.IsNullOrEmpty(msg.OptimalConditions))
                {
                    var optimalConditionsContainer = new BoxContainer
                    {
                        Orientation = BoxContainer.LayoutOrientation.Horizontal,
                        HorizontalAlignment = HAlignment.Center,
                        VerticalAlignment = VAlignment.Center,
                        Margin = new Thickness(0, 4),
                    };

                    var optimalConditionsIcon = new TextureRect
                    {
                        HorizontalAlignment = HAlignment.Center,
                        VerticalAlignment = VAlignment.Center,
                    };

                    optimalConditionsContainer.AddChild(optimalConditionsIcon);

                    var optimalConditionsLabel = new RichTextLabel
                    {
                        Margin = new Thickness(5, 0, 0, 0),
                        MaxWidth = 400,
                    };

                    optimalConditionsLabel.SetMessage(msg.OptimalConditions, defaultColor: Color.Yellow);

                    optimalConditionsContainer.AddChild(optimalConditionsLabel);

                    AlertsContainer.AddChild(optimalConditionsContainer);

                    AlertsContainer.AddChild(new PanelContainer
                    {
                        StyleClasses = { "LowDivider" },
                        Visible = true
                    });
                }
            }
        }
    }
}
