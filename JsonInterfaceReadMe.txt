Assemblies:
	- HeuristicLab.JsonInterface
	- HeuristicLab.JsonInterface.App (Application für das Ausführen von Template-Konfig Kombinationen auf der Kommandozeile)
	- HeuristicLab.JsonInterface.OptimizerIntegration (Importer/Exporter für Templates aus dem Optimizer)

eigenen Converter erstellen:
	- IJsonItemConverter implementieren (oder von BaseConverter ableiten)
	- "Priority", "ConvertableType" Eigenschaften und "Inject", "Extract" Methoden implementieren
	- Converter werden zur Laufzeit gesucht (also egal in welchen Assembly der Converter existiert)
	- es wird immer der Converter mit der höchsten Priorität ausgewählt ( = höchste Zahl)
	- für die Eigenschaft "ConvertableType" kann mit HEAL.Attic der Typ gefunden werden 
	- WICHTIG: immer den "Root"-Converter bei verschachtelte Converter-Aufrufe weiterreichen (verhindert Schleifen bei entsprechende Objektgraphen)


eigenes JsonItem erstellen:
	- IJsonItem implementieren (oder von einer abstrakten JsonItem-Klasse ableiten)
	- für De-/Serialisierung die Methoden "GenerateJObject" (Serialisierung) und "SetJObject" (Deserialisierung) überschreiben
	- für die Validierung (ob z.B.: der Wert eines Items korrekt ist) die Methode "Validate" überschreiben

Ablauf:
	1. Suche nach Convertern (bei der Initialisierung)
	2. passender Converter wird für ein IItem ausgewählt
	3. IItem wird dem Converter übergeben (Extract/Inject)
	4. bei Extract wird ein JsonItem-Baum zurückgegeben


JCGenerator (Name nicht final):
	- erzeugt Templates von einem "IOptimizer"-Objektes
	- ACHTUNG: Templates bestehen immer aus zwei Files (.json, .hl)

JsonTemplateInstantiator:
	- erzeugt ein "IOptimizer"-Objekt aus einem Template (und gegebenfalls einer dazugehörigen konkreten Konfiguration)


CLI Verwendung:
	- HeuristicLab 3.3.exe /start:JsonInterface <Template-Pfad> <Konfig-Pfad> <Ausgabe-Pfad>
	- in Konfig können Parameter des Templates direkt eingefügt und verändert werden
	- "Results" werden bei der Template Erzeugung erst erkannt wenn auch bereits Werte in "Results" enthalten sind