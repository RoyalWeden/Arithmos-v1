using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGameToPanel : MonoBehaviour {

    private void Start() {
        switch(PlayCountingLevel.leveltype) {
            case PlayCountingLevel.LevelType.regular:
                gameObject.AddComponent<NormalCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.addition:
                gameObject.AddComponent<AdditionCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.subtraction:
                gameObject.AddComponent<SubtractionCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.multiplication:
                gameObject.AddComponent<MultiplicationCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.division:
                gameObject.AddComponent<DivisionCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.power:
                gameObject.AddComponent<PowersCountingLevel>();
                break;
            case PlayCountingLevel.LevelType.equation:
                gameObject.AddComponent<EquationsCountingLevel>();
                break;
        }
    }
}
