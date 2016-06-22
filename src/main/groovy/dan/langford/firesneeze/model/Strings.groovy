package dan.langford.firesneeze.model

import javax.persistence.Entity
import javax.persistence.Id
import javax.persistence.Inheritance
import javax.persistence.InheritanceType

@Entity
@Inheritance(strategy=InheritanceType.TABLE_PER_CLASS)
class Strings {

    @Id
    Long Id;

    String defaultText;

    String femaleText;


    @Entity
    class Card extends Strings {
    }

    @Entity
    class Power extends Strings {
    }

    @Entity
    class UI extends Strings {
    }

}

