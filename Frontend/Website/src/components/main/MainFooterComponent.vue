<template>
  <footer>
    <div class="footer_bg">
      <div class="gradient_over"></div>
    </div>
    <div class="container">
      <div class="row move_content">
        <div class="col-lg-4 col-md-12">
          <h5>Địa chỉ</h5>
          <div v-for="(branch, index) in branches" :key="branch.id">
            <b class="mb-1">{{ branch.name }}</b>
            <p class="mb-2">Địa chỉ: {{ branch.address }}, {{ branch.ward }}, {{ branch.district }}<br />{{ branch.province }}</p>
            <p v-for="contact in branch.branch_contact_details" :key="contact.id" class="mb-0"><i :class="contact.branch_contact?.contact_icon" class="me-1"></i> {{ contact.branch_contact?.name }}: {{ contact.value }}</p>
            <hr class="my-3" v-if="index != branches.length - 1" />
          </div>
        </div>
        <div class="col-lg-3 col-md-6 ms-lg-auto">
          <h5>Explore</h5>
          <div class="footer_links">
            <ul>
              <li><a href="index.html">Home</a></li>
              <li><a href="about.html">About Us</a></li>
              <li><a href="room-list-1.html">Rooms &amp; Suites</a></li>
              <li><a href="news-1.html">News &amp; Events</a></li>
              <li><a href="contacts.html">Contacts</a></li>
              <li><a href="about.html">Terms and Conditions</a></li>
            </ul>
          </div>
        </div>
        <div class="col-lg-3 col-md-6">
          <div id="newsletter">
            <h5>Newsletter</h5>
            <div id="message-newsletter"></div>
            <form method="post" action="phpmailer/newsletter_template_email.php" name="newsletter_form" id="newsletter_form">
              <div class="form-group">
                <input type="email" name="email_newsletter" id="email_newsletter" class="form-control" placeholder="Your email" />
                <button type="submit" id="submit-newsletter"><i class="bi bi-send"></i></button>
              </div>
            </form>
            <p>Receive latest offers and promos without spam. You can cancel anytime.</p>
          </div>
        </div>
      </div>
      <!--/row-->
    </div>
    <!--/container-->
    <div class="copy">
      <div class="container">© Paradise - by <a href="https://ansonika.com/paradise/">Ansonika</a></div>
      <div class="container">Rebuild - by <a href="https://www.facebook.com/drakekarto/">Drake Karto</a></div>
    </div>
  </footer>
</template>

<script setup>
import axios from "@/utils/axios";
import image from "/src/assets/main/img/rooms/3.jpg";
import { onMounted, ref } from "vue";
import { toast } from "vue-sonner";

const branches = ref([]);

// Fetch Data
const fetchBranches = async () => {
  try {
    const response = await axios.get("/branch/get-optimal");

    if (response.data.success) {
      branches.value = response.data.data;
    } else toast.error(response.data.message);
  } catch (error) {
    console.error(error);
  }
};

onMounted(() => {
  fetchBranches();
});
</script>
