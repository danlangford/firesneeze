package dan.langford.firesneeze.repo

import dan.langford.firesneeze.model.Card
import org.springframework.data.repository.CrudRepository

interface CardRepository extends CrudRepository<Card, String> {
}
