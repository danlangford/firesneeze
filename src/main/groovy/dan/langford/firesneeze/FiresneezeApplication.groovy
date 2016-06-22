package dan.langford.firesneeze

import org.springframework.boot.SpringApplication
import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.context.annotation.Configuration
import org.springframework.data.jpa.repository.config.EnableJpaRepositories
import org.springframework.data.web.config.EnableSpringDataWebSupport
import org.springframework.web.servlet.config.annotation.EnableWebMvc

@SpringBootApplication
@Configuration
@EnableWebMvc
@EnableJpaRepositories
@EnableSpringDataWebSupport
class FiresneezeApplication {

	static void main(String[] args) {
		SpringApplication.run FiresneezeApplication, args
	}
}
