package dan.langford.firesneeze.model

import javax.persistence.Entity
import javax.persistence.Id

@Entity
class Card {

    @Id
    String id;

    String campaign;

    Character set;

    String rarity;

    Long nameId;

    Long descriptionId;

    String type2;

    String traitIds;

}
